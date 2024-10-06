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
using Amazon.Extensions.NETCore.Setup;
using Amazon;
using Amazon.KeyManagementService;
using Amazon.CognitoIdentityProvider;

namespace RSAAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDefaultAWSOptions(new AWSOptions
            {
                Region = RegionEndpoint.USWest2 
            });
            builder.Services.AddAWSService<IAmazonS3>();
            builder.Services.AddAWSService<IAmazonSecretsManager>();
            builder.Services.AddAWSService<IAmazonKeyManagementService>();
            builder.Services.AddAWSService<IAmazonCognitoIdentityProvider>();

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
            builder.Services.AddSingleton<EncryptionService>(provider =>
            {
                var kmsClient = provider.GetRequiredService<IAmazonKeyManagementService>();
                string keyArn = builder.Configuration["AWS:Key"];
                return new EncryptionService(keyArn, kmsClient);
            });


            builder.Services.AddHttpClient<ILicenseService, LicenseService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISecretService, SecretService>();
            builder.Services.AddScoped<AuthService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
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
                        Array.Empty<string>()
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
