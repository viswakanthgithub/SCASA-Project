using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; 
using Newtonsoft.Json;
using Quartz;
using Rotativa.AspNetCore;
using SCASA.Controllers;
using SCASA.Models.Repositories;
using SCASA.Models.Repositories.Entity;
using SCASA.Models.Repositories.Repos;
using SCASA.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCASA
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
            services.AddDbContext<MyDbContext>(
               options => options.UseSqlServer(Configuration.GetConnectionString("MyConnection")));
            
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();

            services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;

            });
            services.AddQuartz(q =>
           {
                // base quartz scheduler, job and trigger configuration
                q.UseMicrosoftDependencyInjectionScopedJobFactory();
              
                
                // for job 1
                string dailyreportcornjob=Configuration.GetValue<string>("CronJobConfig:DailyReport");
                var jobKey1 = new JobKey("Test1");
                q.AddJob<TestJob1>(opts => opts.WithIdentity(jobKey1));
                q.AddTrigger(opts => opts
                   .ForJob(jobKey1) // link to the HelloWorldJob
                   .WithIdentity("TEST1TRTIGGER")
                   .WithCronSchedule(dailyreportcornjob)); // run every 5 seconds 
                //q.AddTrigger(opts => opts
                //.ForJob(jobKey1)
                //.WithDailyTimeIntervalSchedule(s =>
                //s.WithIntervalInHours(24)
                //.OnEveryDay().StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(22, 00))).WithIdentity("job1-trigger"));
            });
            //services.AddQuartz(q =>
            //{
            //    List<DayOfWeek> reqDays = new List<DayOfWeek>();
            //    DayOfWeek su = DayOfWeek.Sunday;
            //    reqDays.Add(su);
            //    // base quartz scheduler, job and trigger configuration
            //    var JOBKEY2 = new JobKey("Test2");
            //    q.AddJob<TestJob2>(opts => opts.WithIdentity(JOBKEY2));
            //    q.AddTrigger(opts => opts
            //    .ForJob(JOBKEY2)
            //    .WithDailyTimeIntervalSchedule(s =>
            //    s.OnDaysOfTheWeek(reqDays).StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(12, 00))).WithIdentity("job2-trigger"));
            //});
            //services.AddQuartz(q =>
            //{
            //    List<DayOfWeek> reqDays = new List<DayOfWeek>();
            //    DayOfWeek su = DayOfWeek.Sunday;
            //    reqDays.Add(su);
            //    // base quartz scheduler, job and trigger configuration
            //    var JOBKEY2 = new JobKey("Test2");
            //    q.AddJob<TestJob2>(opts => opts.WithIdentity(JOBKEY2));
            //    q.AddTrigger(opts => opts
            //    .ForJob(JOBKEY2)
            //    .WithDailyTimeIntervalSchedule(s =>
            //    s.WithInterval(reqDays).StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(10, 30))));
            //});
            services.AddScoped<IUserMgmtRepo, UserMgmtRepo>();
            services.AddScoped<ICategoryMgmtRepo, CategoryMgmtRepo>();
            services.AddScoped<ICommonRepo, CommonRepo>();
            services.AddScoped<IInventoryRepo, InventoryRepo>();
            services.AddScoped<IInventoryLocationMgmtRepo, InventoryLocationMgmtRepo>();
            services.AddScoped<IInventoryConditionMgmtRepo, InventoryConditionMgmtRepo>();
            services.AddScoped<IInventoryStatusMgmtRepo, InventoryStatusMgmtRepo>();
            services.AddScoped<IPayRollMgmtRepo, PayRollMgmtRepo>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAttendanceMgmtRepo, AttendanceMgmtRepo>();
            services.AddScoped<IMWorkingDaysMgmtRepo, MWorkingDaysMgmtRepo>();
            services.AddScoped<IMonthlyPayRollMgmtRepo, MonthlyPayRollMgmtRepo>();
            services.AddScoped<IPrevilegeMasterMgmtRepo, PrevilegeMasterMgmtRepo>();
            services.AddScoped<IModuleMasterMgmtRepo, ModuleMasterMgmtRepo>();
            services.AddScoped<IHolidayMgmtRepo, HolidayMgmtRepo>();
            services.AddScoped<IShiftMasterMgmtRepo, ShiftMasterMgmtRepo>();
            services.AddScoped<ICompanyMgmtRepo, CompanyMgmtRepo>();
            services.AddScoped<IGSTMasterMgmtRepo, GSTMasterMgmtRepo>();
            services.AddScoped<ICustomerMgmtRepo, CustomerMgmtRepo>();
            services.AddScoped<ISalesDetailsMgmtRepo, SalesDetailsMgmtRepo>();
            services.AddScoped<ISalesMasterMgmtRepo, SalesMasterMgmtRepo>();
            services.AddScoped<IFinanceMgmtRepo, FinanceMgmtRepo>();
            services.AddScoped<IStaffLoansMgmtRepo, StaffLoansMgmtRepo>();
            services.AddScoped<IMRPFactorMgmtRepo, MRPFactorMgmtRepo>();
            services.AddScoped<IRecordExceptionRepo, RecordExceptionRepo>();
            services.AddScoped<IReportsMgmgtRepo, ReportsMgmgtRepo>();

            services.AddScoped<IUserMgmtService, UserMgmtService>();
            services.AddScoped<ICategoryMgmtService, CategoryMgmtService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IInventoryLocationMgmtService, InventoryLocationMgmtService>();
            services.AddScoped<IInventoryConditionMgmtService, InventoryConditionMgmtService>();
            services.AddScoped<IInventoryStatusMgmtService, InventoryStatusMgmtService>();
            services.AddScoped<IpayRollMgmtService, payRollMgmtService>();
            services.AddScoped<IAttendanceMgmtService, AttendanceMgmtService>();
            services.AddScoped<IMWorkingDaysMgmtService, MWorkingDaysMgmtService>();
            services.AddScoped<IMonthlyPayrollService, MonthlyPayrollService>();
            services.AddScoped<IPrevilegeMasterMgmtService, PrevilegeMasterMgmtService>();
            services.AddScoped<IModuleMgmtService, ModuleMgmtService>();
            services.AddScoped<IHolidayMgmtService, HolidayMgmtService>();
            services.AddScoped<IShiftMasterMgmtService, ShiftMasterMgmtService>();
            services.AddScoped<ICompanyMgmtService, CompanyMgmtService>();
            services.AddScoped<IGSTMasterMgmtService, GSTMasterMgmtService>();
            services.AddScoped<ICustomerMgmtService, CustomerMgmtService>();
            services.AddScoped<ISalesMasterMgmtService, SalesMasterMgmtService>();
            services.AddScoped<ISalesDetailsMgmtService, SalesDetailsMgmtService>();
            services.AddScoped<IFinanceMgmtService, FinanceMgmtService>();
            services.AddScoped<IStaffLoansMgmtService, StaffLoansMgmtService>();
            services.AddScoped<IMRPFactorMgmtService, MRPFactorMgmtService>();
            services.AddScoped<IRecordExceptionService, RecordExceptionService>();
            services.AddScoped<IReportsMgmtService, ReportsMgmtService>();

            services.AddMvc().AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(540);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

            app.UseSession();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Authenticate}/{action=Login}/{id?}");
            });
            RotativaConfiguration.Setup(env);
        }
    }
}
