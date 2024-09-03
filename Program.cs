using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RSAAPI.Abstracts;
using RSAAPI.Database;
using RSAAPI.Services;
using Microsoft.Data.SqlClient;
using Amazon.S3;
using Amazon.SecretsManager;
using Newtonsoft.Json.Linq;
using System.Security;
using Amazon.Extensions.NETCore.Setup;
using Amazon;

namespace RSAAPI
{
    public class Program
    {
        public static byte[] Base64UrlDecode(string base64Url)
        {
            // Replace URL-specific characters
            string base64 = base64Url.Replace('-', '+').Replace('_', '/');

            // Pad with '=' to make the string length a multiple of 4
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            // Decode the Base64 string to a byte array
            return Convert.FromBase64String(base64);
        }


        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
            builder.Services.AddDefaultAWSOptions(new AWSOptions
            {
                Region = RegionEndpoint.USWest2 // Specify your region here
            });
            builder.Services.AddAWSService<IAmazonS3>();
            builder.Services.AddAWSService<IAmazonSecretsManager>();

            // Configure JWT Bearer Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = $"https://cognito-idp.{builder.Configuration["AWS:Region"]}.amazonaws.com/{builder.Configuration["AWS:UserPoolId"]}",
                        ValidAudience = builder.Configuration["AWS:ClientId"],
                        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                        {
                            // Fetch the JSON Web Key Set (JWKS) from the Cognito issuer.
                            var jwksUrl = $"{parameters.ValidIssuer}/.well-known/jwks.json";
                            var httpClient = new HttpClient();
                            var response = httpClient.GetAsync(jwksUrl).Result;
                            var json = response.Content.ReadAsStringAsync().Result;
                            var keys = new JsonWebKeySet(json).Keys;
                            return keys;
                        }

                    };
                });

            builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError("Authentication failed: {Exception}", context.Exception.ToString());
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogInformation("Token validated for {Principal}", context.Principal);

                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
            });


            // Add services to the container.
            builder.Services.AddHttpClient<ILicenseService, LicenseService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISecretService, SecretService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "RSAAPI"
                });
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });
            });


            builder.Services.AddDbContext<RSAContext>((serviceProvider, options) =>
            {
                string partialConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                var secretManagerService = serviceProvider.GetRequiredService<ISecretService>();
                var secretJson = secretManagerService.GetSecretValueAsync("prod/Store").GetAwaiter().GetResult();
                var secret = JObject.Parse(secretJson);   

                var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(partialConnectionString)
                {
                    DataSource = secret["SOURCE"]?.ToString(),  
                    Password = secret["PASS"]?.ToString()    
                };

                options.UseSqlServer(sqlConnectionStringBuilder.ConnectionString);
            });

            var app = builder.Build();

            app.UseCors("AllowAllOrigins");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapGet("/health", () => Results.Ok("Application is healthy"));
            app.MapControllers();
            app.Run();
        }
    }
}
