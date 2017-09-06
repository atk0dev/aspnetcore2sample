using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using WebApplication1.Data;
using WebApplication1.Repositories;
using WebApplication1.Services;

namespace WebApplication1
{
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        private IConfigurationRoot _configurationRoot;

        public Startup(IHostingEnvironment hostingEnvironment)
        {
            _configurationRoot = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                //.AddJsonFile("appsettings.json")
                //.AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{hostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            Configuration = _configurationRoot;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(_configurationRoot.GetConnectionString("DefaultConnection")));

            //services.AddTransient<ICategoryRepository, MockCategoryRepository>();
            //services.AddTransient<IPieRepository, MockPieRepository>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif

            services.AddTransient<IPieRepository, PieRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ShoppingCart>(sp => ShoppingCart.GetCart(sp));
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddScoped<ICityInfoRepository, CityInfoRepository>();

            services.AddMvc()
                .AddMvcOptions(o => o.OutputFormatters.Add(
                    new XmlDataContractSerializerOutputFormatter()));

            services.AddMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            //loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            else
            {
                app.UseExceptionHandler("/AppException");
            }

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Models.City, Dto.CityWithoutPointsOfInterestDto>();
                cfg.CreateMap<Models.City, Dto.CityDto>();
                cfg.CreateMap<Models.PointOfInterest, Dto.PointOfInterestDto>();
                cfg.CreateMap<Dto.PointOfInterestForCreationDto, Models.PointOfInterest>();
                cfg.CreateMap<Dto.PointOfInterestForUpdateDto, Models.PointOfInterest>();
                cfg.CreateMap<Models.PointOfInterest, Dto.PointOfInterestForUpdateDto>();
            });

            app.UseStaticFiles();
            app.UseSession();
            //app.UseIdentity();
            app.UseAuthentication();

            //app.UseMvcWithDefaultRoute();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "piedetails",
                    template: "Pie/{action}/{id?}",
                    defaults: new { Controller = "Pie", action = "List" });

                routes.MapRoute(
                    name: "categoryfilter",
                    template: "Pie/{action}/{category?}",
                    defaults: new { Controller = "Pie", action = "List" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");


            });


            DbInitializer.Seed(app);
        }
    }
}
