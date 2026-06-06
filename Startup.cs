using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using App.Services;
using System.Globalization;

namespace App
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSingleton<SigacarStore>();
            services.AddSingleton<PerfilFuzzyService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var culturaBrasil = new CultureInfo("pt-BR");
            CultureInfo.DefaultThreadCurrentCulture = culturaBrasil;
            CultureInfo.DefaultThreadCurrentUICulture = culturaBrasil;

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
