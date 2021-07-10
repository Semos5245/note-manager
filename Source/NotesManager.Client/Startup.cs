using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using NotesManager.Client.Data;
using NotesManager.Client.Services;
using System;
using System.Threading.Tasks;

namespace NotesManager.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMudServices();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddElectron();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration["DbConnections:SqlServerConnection"]).UseLazyLoadingProxies());

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITextFileService, TextFileService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            if (Convert.ToBoolean(Configuration["GeneralSettings:UseElectron"]) && HybridSupport.IsElectronActive)
                Task.Run(async () => await BootstrapApp());
        }

        private static Task BootstrapApp()
        {
            return Task.Run(async () =>
            {
                var windowOptions = new BrowserWindowOptions
                {
                    Icon = @"D:\Projects\Mine\C#\NotesManager\NotesManager.Client\wwwroot\notes.ico",
                    Title = "Notes Center",
                    Transparent = true,
                    HasShadow = true,
                    Center = true,
                    Maximizable = false,
                    Resizable = true,
                    Width = 1300,
                    Height = 800
                };

                var window = await Electron.WindowManager.CreateWindowAsync(windowOptions);

                window.OnClosed += () => { Electron.App.Quit(); };
            });
        }
    }
}
