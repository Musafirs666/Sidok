using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using OpenSearch.Client;
using Repository;
using Repository.Interface;
using Service;
using Service.Interfaces;
using System.Data.SqlClient;
using OpenSearch.Net;
using System.Data;

namespace Sidok
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Ambil konfigurasi OpenSearch dari appsettings.json
            var opensearchUrl = configuration["OpenSearch:Url"];
            var username = configuration["OpenSearch:Username"];
            var password = configuration["OpenSearch:Password"];
            var defaultIndex = configuration["OpenSearch:DefaultIndex"];

            var settings = new ConnectionSettings(new Uri(opensearchUrl))
                .DefaultIndex(defaultIndex)
                .BasicAuthentication(username, password)
                .ServerCertificateValidationCallback(CertificateValidations.AllowAll);

            var openSearchClient = new OpenSearchClient(settings);

            // Add services to the container.
            builder.Services.AddSingleton<IOpenSearchClient>(openSearchClient);
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IDokterRepository, DokterRepository>();
            builder.Services.AddScoped<IDokterService, DokterService>();
            builder.Services.AddScoped<IPoliRepository, PoliRepository>();
            builder.Services.AddScoped<IPoliService, PoliService>();
            builder.Services.AddScoped<ISpesialisasiService, SpesialisasiService>();
            builder.Services.AddScoped<ISpesialisasiRepository, SpesialisasiRepository>();
            builder.Services.AddScoped<IDokterPoliService, DokterPoliService>();
            builder.Services.AddScoped<IDokterPoliRepository, DokterPoliRepository>();
            builder.Services.AddScoped<IOpensearchDokterService, OpensearchDokterService>();
            builder.Services.AddScoped<IOpensearchDokterRepository, OpensearchDokterRepository>();

            // Tambahkan konfigurasi untuk Dapper
            builder.Services.AddScoped<IDbConnection>(sp =>
                new SqlConnection(configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dokter}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
