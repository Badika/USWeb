using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UpsCoolWeb.Components.Extensions;
using UpsCoolWeb.Components.Logging;
using UpsCoolWeb.Components.Mail;
using UpsCoolWeb.Components.Mvc;
using UpsCoolWeb.Components.Security;
using UpsCoolWeb.Controllers;
using UpsCoolWeb.Data.Core;
using UpsCoolWeb.Data.Logging;
using UpsCoolWeb.Data.Migrations;
using UpsCoolWeb.Objects;
using UpsCoolWeb.Resources;
using UpsCoolWeb.Services;
using UpsCoolWeb.Validators;
using NonFactors.Mvc.Grid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UpsCoolWeb.Web
{
    public class Startup
    {
        private IConfiguration Config { get; }

        public Startup(IHostingEnvironment env)
        {
            Dictionary<String, String> config = new Dictionary<String, String>();
            config.Add("Application:Path", env.ContentRootPath);
            config.Add("Application:Env", env.EnvironmentName);

            Config = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables("ASPNETCORE_")
                .AddInMemoryCollection(config)
                .AddJsonFile("configuration.json")
                .AddJsonFile($"configuration.{env.EnvironmentName.ToLower()}.json", optional: true)
                .Build();

            RegisterViewResources();
        }
        public void Configure(IApplicationBuilder app)
        {
            RegisterMiddleware(app);
            RegisterMvc(app);

            UpdateDatabase(app);
        }
        public void ConfigureServices(IServiceCollection services)
        {
            RegisterMvc(services);
            RegisterLogging(services);
            RegisterServices(services);
            RegisterLowercaseUrls(services);
            RegisterSecureResponse(services);
        }

        public void RegisterViewResources()
        {
            Type[] types = typeof(BaseView).Assembly.GetTypes();

            foreach (Type view in types.Where(type => typeof(BaseView).IsAssignableFrom(type)))
            {
                Type type = view;

                while (typeof(BaseView).IsAssignableFrom(type.BaseType))
                {
                    Resource.Set(view.Name).Inherit(Resource.Set(type.BaseType.Name));

                    type = type.BaseType;
                }
            }
        }

        public void RegisterMvc(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddMvcOptions(options => options.Filters.Add(typeof(LanguageFilter)))
                .AddMvcOptions(options => options.Filters.Add(typeof(AuthorizationFilter)))
                .AddMvcOptions(options => ModelMessagesProvider.Set(options.ModelBindingMessageProvider))
                .AddRazorOptions(options => options.ViewLocationExpanders.Add(new ViewLocationExpander()))
                .AddViewOptions(options => options.ClientModelValidatorProviders.Add(new DateValidatorProvider()))
                .AddMvcOptions(options => options.ModelMetadataDetailsProviders.Add(new DisplayMetadataProvider()))
                .AddViewOptions(options => options.ClientModelValidatorProviders.Add(new NumberValidatorProvider()))
                .AddMvcOptions(options => options.ModelBinderProviders.Insert(4, new TrimmingModelBinderProvider()));

            services.AddAuthentication("Cookies").AddCookie(authentication =>
            {
                authentication.Cookie.Name = Config["Cookies:Auth:Name"];
                authentication.Events = new AuthenticationEvents();
            });

            services.AddMvcGrid(filters =>
            {
                filters.BooleanFalseOptionText = () => Resource.ForString("No");
                filters.BooleanTrueOptionText = () => Resource.ForString("Yes");
            });
        }
        public void RegisterLogging(IServiceCollection services)
        {
            if (Config["Application:Env"] != EnvironmentName.Development)
                services.AddLogging(builder => builder.AddProvider(new FileLoggerProvider(Config)));
            else
                services.AddLogging(builder => builder.AddConsole());
        }
        public void RegisterServices(IServiceCollection services)
        {
            services.AddSession();
            services.AddSingleton(Config);

            services.AddTransient<Configuration>();
            services.AddTransient<DbContext, Context>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<Context>(options => options.UseSqlServer(Config["Data:Connection"]));

            services.AddTransient<IAuditLogger>(provider =>
                new AuditLogger(provider.GetService<DbContext>(),
                provider.GetRequiredService<IHttpContextAccessor>().HttpContext?.User?.Id()));

            services.AddSingleton<IHasher, BCrypter>();
            services.AddSingleton<IMailClient, SmtpMailClient>();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IValidationAttributeAdapterProvider, ValidationAdapterProvider>();

            services.AddSingleton<IAuthorization>(provider =>
                new Authorization(typeof(BaseController).Assembly, provider));

            Language[] supported = Config.GetSection("Languages:Supported").Get<Language[]>();
            services.AddSingleton<ILanguages>(new Languages(Config["Languages:Default"], supported));

            String map = File.ReadAllText(Path.Combine(Config["Application:Path"], Config["SiteMap:Path"]));
            services.AddSingleton<ISiteMap>(provider => new SiteMap(map, provider.GetService<IAuthorization>()));

            services.AddTransientImplementations<IService>();
            services.AddTransientImplementations<IValidator>();
        }
        public void RegisterLowercaseUrls(IServiceCollection services)
        {
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        }
        public void RegisterSecureResponse(IServiceCollection services)
        {
            services.Configure<CookieTempDataProviderOptions>(provider => provider.Cookie.Name = Config["Cookies:TempData:Name"]);
            services.Configure<SessionOptions>(session => session.Cookie.Name = Config["Cookies:Session:Name"]);
            services.Configure<AntiforgeryOptions>(antiforgery =>
            {
                antiforgery.Cookie.Name = Config["Cookies:Antiforgery:Name"];
                antiforgery.FormFieldName = "_Token_";
            });
        }

        public void RegisterMiddleware(IApplicationBuilder app)
        {
            if (Config["Application:Env"] == EnvironmentName.Development)
                app.UseMiddleware<DeveloperExceptionPageMiddleware>();
            else
                app.UseMiddleware<ErrorPagesMiddleware>();

            app.UseMiddleware<SecureHeadersMiddleware>();

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = (response) =>
                {
                    response.Context.Response.Headers["Cache-Control"] = "max-age=8640000";
                }
            });
            app.UseSession();
        }
        public void RegisterMvc(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute("MultiArea", "{language}/{area:exists}/{controller}/{action=Index}/{id:int?}");
                routes.MapRoute("DefaultArea", "{area:exists}/{controller}/{action=Index}/{id:int?}");
                routes.MapRoute("Multi", "{language}/{controller}/{action=Index}/{id:int?}");
                routes.MapRoute("Default", "{controller}/{action=Index}/{id:int?}");
                routes.MapRoute("Home", "{controller=Home}/{action=Index}");
            });
        }

        public void UpdateDatabase(IApplicationBuilder app)
        {
            using (Configuration configuration = app.ApplicationServices.GetService<Configuration>())
                configuration.UpdateDatabase();
        }
    }
}
