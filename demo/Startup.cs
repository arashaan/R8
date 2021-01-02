using System;
using Humanizer.Localisation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using R8.AspNetCore.Demo.Pages;
using R8.AspNetCore.Demo.Services.Globalization;
using R8.AspNetCore.FileHandlers;
using R8.AspNetCore.Localization;
using R8.AspNetCore.ModelBinders;
using R8.AspNetCore.Routing;
using R8.Lib;
using R8.Lib.Localization;

using SixLabors.ImageSharp.Formats.Png;

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace R8.AspNetCore.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{webHostEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables()
                .SetBasePath(webHostEnvironment.ContentRootPath)
                .Build();
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(this.Configuration);

            var appDbContextConnectionString = Configuration
                .GetConnectionString("ApplicationDbContextConnection");

            // services.AddCustomPooledDbContextFactory<ApplicationDbContext>(options =>
            //     options.ConnectionString = appDbContextConnectionString);

            services.AddLocalization(options => options.ResourcesPath = nameof(Resources))
                .Configure<RequestLocalizationOptions>(options =>
                {
                    options.ConfigureRequestLocalization();
                    options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider
                    {
                        RouteDataStringKey = LanguageRouteConstraint.Key,
                        Options = options
                    });
                });

            services
                .AddMvc(options =>
                {
                    options.ModelBinderProviders.Insert(0, new StringModelBinderProvider());
                    options.ModelBinderProviders.Insert(0, new DateTimeUtcModelBinderProvider());
                    options.EnableEndpointRouting = false;
                    options.SuppressAsyncSuffixInActionNames = false;
                    options.ValueProviderFactories.Insert(0, new SeparatedQueryStringValueProviderFactory());

                    options.Conventions.Add(new LocalizeActionRouteModelConvention());
                    //options.Filters.Add(new MiddlewareFilterAttribute(typeof(LocalizationPipeline)));
                });
            // .AddMvcLocalization()
            // .AddViewLocalization()
            // .AddViewOptions(options =>
            // {
            //     options.HtmlHelperOptions.ClientValidationEnabled = true;
            //     options.HtmlHelperOptions.Html5DateRenderingMode = Html5DateRenderingMode.Rfc3339;
            // })
            //
            // .AddDataAnnotationsLocalization(options =>
            // {
            //     options.DataAnnotationLocalizerProvider = (_, factory) =>
            //         factory.Create(typeof(Resources));
            // })
            // .AddControllersAsServices()
            // .AddJsonOptions(options =>
            // {
            //     options.JsonSerializerOptions.PropertyNamingPolicy = null;
            //     options.JsonSerializerOptions.DictionaryKeyPolicy = null;
            // });

            // services.AddDistributedMemoryCache();
            services.AddMemoryCache();

            services.AddHttpContextAccessor();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IApplicationBuilder, ApplicationBuilder>();
            services.AddScoped(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            services.AddScoped<ICulturalizedUrlHelper, CulturalizedUrlHelper>();
            // services.AddTransient<ApplicationDbContext>();

            //services.AddLocalizer((serviceProvider, config) =>
            //{
            //    using var scope = serviceProvider.CreateScope();
            //    var request = scope.ServiceProvider.GetService<IOptions<RequestLocalizationOptions>>();

            //    config.SupportedCultures = request.Value.SupportedCultures.ToList();
            //    config.UseMemoryCache = true;
            //    config.Provider = new LocalizerCustomProvider
            //    {
            //        DictionaryAsync = async () =>
            //        {
            //            var dictionary = new Dictionary<string, LocalizerContainer>();
            //            await using var connection = new SqlConnection(appDbContextConnectionString);
            //            const string queryRaw = @"SELECT [Key], [IsDeleted], [Name]
            //                             FROM [Translations]
            //                             WHERE [IsDeleted] <> CAST(1 AS bit)
            //                             ORDER BY [Key]";
            //            var cmd = new SqlCommand(queryRaw, connection);
            //            connection.Open();
            //            var reader = await cmd.ExecuteReaderAsync();
            //            while (reader.HasRows)
            //            {
            //                while (reader.Read())
            //                {
            //                    var key = reader.GetString("Key");
            //                    var containerJson = reader.GetString("Name");
            //                    var container = LocalizerContainer.Deserialize(containerJson);
            //                    dictionary.Add(key, container);
            //                }
            //                reader.NextResult();
            //            }

            //            connection.Close();
            //            return dictionary;
            //        }
            //    };
            //});
            services.AddLocalizer((serviceProvider, config) =>
            {
                using var scope = serviceProvider.CreateScope();
                var request = scope.ServiceProvider.GetService<IOptions<RequestLocalizationOptions>>();
                if (request == null)
                    throw new NullReferenceException(nameof(RequestLocalizationOptions));

                var localizationOptions = request.Value;

                config.SupportedCultures = localizationOptions.SupportedCultures.ToList();
                config.UseMemoryCache = true;
                config.CacheSlidingExpiration = TimeSpan.FromDays(3);
                config.Provider = new LocalizerJsonProvider
                {
                    Folder = "E:/Work/Develope/R8/test/R8.Test.Constants/Assets/Dictionary",
                    FileName = "dic",
                };
            });

            services.AddFileHandlers((_, options) =>
            {
                options.Path = "/uploads";
                options.HierarchicallyDateFolders = true;
                options.SaveAsRealName = true;
                options.OverwriteExistingFile = true;
                options.Runtimes.Add(new FileHandlerImageRuntime
                {
                    ImageEncoder = new PngEncoder()
                });
            });

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // app.UseFileServer();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(typeof(ErrorModel).GetPagePath());
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);
            ServiceActivator.Configure(app.ApplicationServices);

            app.UseLocalizer();
            app.UseResponse();
            app.UseFileHandlers();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}