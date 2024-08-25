using RSAAPI.Abstracts;
using RSAAPI.Services;
using System.Net.Security;

namespace RSAAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpClient<ILicenseService, LicenseService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
