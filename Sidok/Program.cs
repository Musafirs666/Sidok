using Repository;
using Repository.Interface;
using Service;
using Service.Interfaces;

namespace Sidok
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IDokterRepository, DokterRepository>();
            builder.Services.AddScoped<IDokterService, DokterService>();
            builder.Services.AddScoped<IPoliRepository, PoliRepository>();
            builder.Services.AddScoped<IPoliService, PoliService>();
            builder.Services.AddScoped<ISpesialisasiService, SpesialisasiService>();
            builder.Services.AddScoped<ISpesialisasiRepository, SpesialisasiRepository>();
            builder.Services.AddScoped<IDokterPoliService, DokterPoliService>();
            builder.Services.AddScoped<IDokterPoliRepository, DokterPoliRepository>();

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
