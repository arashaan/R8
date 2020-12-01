using System;
using System.Linq;

using Humanizer.Localisation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
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

namespace R8.AspNetCore.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCustomPooledDbContextFactory<ApplicationDbContext>(options =>
            //    options.ConnectionString =
            //        "Data Source=mssql10.trwww.com;Initial Catalog=ecohosco__DB;User ID=eCoDbaSeHOs;Password=?fs7e9M9;MultipleActiveResultSets=True;App=EntityFramework;integrated security=False");

            services.AddLocalization(options => options.ResourcesPath = nameof(Resources))
                .Configure<RequestLocalizationOptions>(options =>
                {
                    options.ConfigureRequestLocalization();
                    options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider
                    {
                        RouteDataStringKey = LanguageRouteConstraint.Key,
                        Options = options
                    });

                    //options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
                    //{
                    //    var userLang = context.Request.Headers[HeaderNames.AcceptLanguage].ToString();
                    //    var firstLang = userLang.Split(",").FirstOrDefault();
                    //    var defaultLang = string.IsNullOrEmpty(firstLang) ? "tr" : firstLang;
                    //    return Task.FromResult(new ProviderCultureResult(defaultLang, defaultLang));
                    //}));
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
                })
                .AddMvcLocalization()
                .AddViewLocalization()
                .AddViewOptions(options =>
                {
                    options.HtmlHelperOptions.ClientValidationEnabled = true;
                    options.HtmlHelperOptions.Html5DateRenderingMode = Html5DateRenderingMode.Rfc3339;
                })

                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (_, factory) =>
                        factory.Create(typeof(Resources));
                })
                .AddControllersAsServices()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
                });

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
            //    var localizationOptions = scope.ServiceProvider.GetService<IOptions<RequestLocalizationOptions>>().Value;
            //    config.SupportedCultures = localizationOptions.SupportedCultures.ToList();
            //    config.UseMemoryCache = true;
            //    config.Provider = new LocalizerCustomProvider
            //    {
            //        DictionaryAsync = async provider =>
            //        {
            //            await using var dbContext = serviceProvider.GetService<ApplicationDbContext>();
            //            return await dbContext.Translation
            //                .AsNoTracking()
            //                .Cacheable()
            //                .ToDictionaryAsync(x => x.Key, x => x.Name);
            //        },
            //    };
            //});
            services.AddLocalizer((serviceProvider, config) =>
            {
                using var scope = serviceProvider.CreateScope();
                var localizationOptions = scope.ServiceProvider.GetService<IOptions<RequestLocalizationOptions>>().Value;

                config.SupportedCultures = localizationOptions.SupportedCultures.ToList();
                config.UseMemoryCache = true;
                // config.CacheSlidingExpiration = TimeSpan.FromDays(3);
                config.Provider = new LocalizerJsonProvider
                {
                    Folder = "E:/Work/Develope/Ecohos/Ecohos.Presentation/Dictionary",
                    FileName = "dic",
                };
            });

            services.AddFileHandlers((_, options) =>
            {
                options.Path = "/uploads";
                options.HierarchicallyDateFolders = true;
                options.SaveAsRealName = false;
                options.OverwriteExistingFile = false;
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
            app.UseFileServer();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(typeof(ErrorModel).GetPagePath());
                // app.UseHsts();
                // app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            // app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);
            ServiceActivator.Configure(app.ApplicationServices);

            app.UseLocalizer();
            app.UseFileHandlers();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}