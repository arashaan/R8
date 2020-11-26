using EFCoreSecondLevelCacheInterceptor;

using Humanizer.Localisation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using R8.AspNetCore.Demo.Pages;
using R8.AspNetCore.Demo.Services.Database;
using R8.AspNetCore.Demo.Services.Globalization;
using R8.AspNetCore.FileHandlers;
using R8.AspNetCore.Localization;
using R8.AspNetCore.ModelBinders;
using R8.AspNetCore.Routing;
using R8.Lib;
using R8.Lib.Localization;

using SixLabors.ImageSharp.Formats.Png;

using System.Linq;

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

            //services.AddLocalizer(serviceProvider =>
            //{
            //    using var scope = serviceProvider.CreateScope();
            //    var request = scope.ServiceProvider.GetService<IOptions<RequestLocalizationOptions>>();
            //    var configuration = new LocalizerCustomProvider
            //    {
            //        ExpressionAsync = provider =>
            //        {
            //            using var scope2 = provider.CreateScope();
            //            using var dbContext = scope2.ServiceProvider.GetService<ApplicationDbContext>();
            //            return dbContext.Translation
            //                .AsNoTrackingWithIdentityResolution()
            //                .Cacheable()
            //                .ToDictionaryAsync(x => x.Key, x => x.Name);
            //        },
            //        DefaultCulture = request.Value.DefaultRequestCulture.Culture,
            //        SupportedCultures = request.Value.SupportedCultures.ToList()
            //    };
            //    return configuration;
            //});
            services.AddLocalizer(serviceProvider =>
            {
                using var scope = serviceProvider.CreateScope();

                var request = scope.ServiceProvider.GetService<IOptions<RequestLocalizationOptions>>();
                var configuration = new LocalizerJsonProvider
                {
                    Folder = "E:/Work/Develope/Ecohos/Ecohos.Presentation/Dictionary",
                    FileName = "dic",
                    DefaultCulture = request.Value.DefaultRequestCulture.Culture,
                    SupportedCultures = request.Value.SupportedCultures.ToList()
                };
                return configuration;
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

            services.AddFileHandlers((environment, options) =>
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