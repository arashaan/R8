using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using R8.AspNetCore.FileHandlers;

using SixLabors.ImageSharp.Formats.Png;

using System.IO;

namespace R8.AspNetCore.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFileHandlers((environment, options) =>
            {
                options.Folder = "/uploads";
                options.HierarchicallyFolderNameByDate = true;
                options.RealFilename = false;
                options.OverwriteFile = false;
                options.Runtimes.Add(new FileHandlerImageRuntime
                {
                    ImageEncoder = new PngEncoder()
                });
                options.Runtimes.Add(new FileHandlerPdfRuntime
                {
                    GhostScriptDllPath = environment.ContentRootPath + "/gsdll64.dll",
                });
            });

            services.AddRazorPages();
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
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseFileHandlers();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}