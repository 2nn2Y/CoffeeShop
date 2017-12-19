using System.Reflection;
using System.Web;
using System.Web.Routing;
using Abp;
using Abp.Configuration.Startup;
using Abp.Hangfire;
using Abp.Hangfire.Configuration;
using Abp.IO;
using Abp.Modules;
using Abp.Runtime.Caching.Redis;
using Abp.Web.Mvc;
using Abp.Web.SignalR;
using Abp.Zero.Configuration;
using Castle.MicroKernel.Registration;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.MySql;
using Hangfire.MySql.src;
using Microsoft.Owin.Security;
using YT.ThreeData;
using YT.Web.Routing;
using YT.WebApi;
using YT.WebApi.Controllers;

namespace YT.Web
{
    /// <summary>
    /// Web module of the application.
    /// This is the most top and entrance module that depends on others.
    /// </summary>
    [DependsOn(
        typeof(AbpWebMvcModule),
        typeof(AbpZeroOwinModule),
        typeof(YtDataModule),
        typeof(YtApplicationModule),
        typeof(YtWebApiModule),
        typeof(AbpWebSignalRModule),
        typeof(AbpRedisCacheModule), //AbpRedisCacheModule dependency can be removed if not using Redis cache
        typeof(AbpHangfireModule))] //AbpHangfireModule dependency can be removed if not using Hangfire
    public class AbpZeroTemplateWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Use database for language management
         //   Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();
            Configuration.Modules.AbpWeb().AntiForgery.IsEnabled = false;
            //Configure navigation/menu
            //  Configuration.Navigation.Providers.Add<AppNavigationProvider>();//SPA!
            //  Configuration.Navigation.Providers.Add<FrontEndNavigationProvider>();
            //   Configuration.Navigation.Providers.Add<MpaNavigationProvider>();//MPA!

            //  Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = WebUrlService.WebSiteRootAddress;
            
            //Uncomment these lines to use HangFire as background job manager.
            Configuration.BackgroundJobs.UseHangfire(
                configuration =>
                {
                  //  configuration.GlobalConfiguration.UseMySqlStorage("Default");
                configuration.GlobalConfiguration.UseMemoryStorage();
            });

            //Uncomment this line to use Redis cache instead of in-memory cache.
            //Configuration.Caching.UseRedis();
        }

        public override void Initialize()
        {
            //Dependency Injection
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            IocManager.IocContainer.Register(
                Component
                    .For<IAuthenticationManager>()
                    .UsingFactoryMethod(() => HttpContext.Current.GetOwinContext().Authentication)
                    .LifestyleTransient()
                );

            //Areas
          //  AreaRegistration.RegisterAllAreas();

            //Routes
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        public override void PostInitialize()
        {
            var server = HttpContext.Current.Server;
            var appFolders = IocManager.Resolve<AppFolders>();
            appFolders.ImageFolder = server.MapPath("~/Files");
            appFolders.TempFolder = server.MapPath("~/Temp/Downloads");
            appFolders.LogsFolder = server.MapPath("~/App_Data/Logs");
            try
            {
                DirectoryHelper.CreateIfNotExists(appFolders.ImageFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.TempFolder);

            } catch { }


            var background = IocManager.Resolve<BackgroundManager>();
            var controller = IocManager.Resolve<SignController>();
            //同步订单  每12分钟一次
            RecurringJob.AddOrUpdate(() => background.GenderOrder(), "0/12 * * * *");
            RecurringJob.AddOrUpdate(() => background.GenderTodayOrder(), "0 0/23 * * *");
            // RecurringJob.AddOrUpdate(() => background.GenderMonthOrder(), "0/59 * * * *");
            //  同步报警信息  15分钟一次
            RecurringJob.AddOrUpdate(() => controller.GenderWarning(), "0/15 * * * *");
            //同步微信图片 1小时一次
            RecurringJob.AddOrUpdate(()=>controller.GenderImage(), "0/59 * * * *");
            //发送报警信息 15分钟一次
            RecurringJob.AddOrUpdate(()=>controller.ForWarn(), "0/15 * * * *");
        }
    }
}
