using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arragro.Core.EntityFrameworkCore.Extensions;
using Arragro.ObjectHistory.Client.Extensions;
using Arragro.ObjectHistory.Web;
using Arragro.ObjectHistory.WebExample.Core.Entities;
using Arragro.ObjectHistory.WebExample.Core.Interfaces;
using Arragro.ObjectHistory.WebExample.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arragro.ObjectHistory.WebExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void ConfigureObjectHistoryAuthenticationAndPolicies(IServiceCollection services)
        {
            services.AddAuthentication()
                .AddCookie();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ArragroObjectHistoryPolicy", policy =>
                {
                    policy.RequireAssertion(x => true);
                    policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
                });
                options.AddPolicy("ArragroObjectHistoryGlobalLogPolicy", policy =>
                {
                    policy.RequireAssertion(x => true);
                    policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
                });
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DemoDbContext>(
                //optionsBuilder => optionsBuilder.UseInMemoryDatabase("InMemoryDb"));
                optionsBuilder => optionsBuilder.UseSqlServer("data source=localhost;initial catalog=ArragroObjectTracker;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework;", sqlOptions => sqlOptions.EnableRetryOnFailure(3)));
            
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            ConfigureObjectHistoryAuthenticationAndPolicies(services);
            services.AddArragroObjectHistoryClient<ObjectLogsSecurityAttribute>(Configuration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<ITrainingSessionRepository, EFTrainingSessionRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                var demoDbContext = app.ApplicationServices.GetService<DemoDbContext>();
                var repository = app.ApplicationServices.GetService<ITrainingSessionRepository>();
                CreateAndMigrateDatabase(demoDbContext, repository);

                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(("/error/exception"));
                app.UseStatusCodePagesWithReExecute("/error/{0}");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute("spa-fallback", new { controller = "Session", action = "SpaIndex" });
            });
        }

        public void CreateAndMigrateDatabase(
            DemoDbContext demoDbContext,
            ITrainingSessionRepository trainingSessionRepository)
        {
            var demoDbExists = demoDbContext.Exists();

            if (!demoDbExists || (!demoDbContext.AllMigrationsApplied()))
            {
                demoDbContext.Database.Migrate();
                if (!demoDbExists)
                {
                    InitializeDatabaseAsync(trainingSessionRepository).Wait();
                }
            }
        }

        public async Task InitializeDatabaseAsync(ITrainingSessionRepository repo)
        {
            var sessionList = await repo.ListAsync();
            if (!sessionList.Any())
            {
                var sessions = GetInitSession();
                foreach(var session in sessions) 
                {
                    await repo.AddAsync(session);
                }
            }
        }

        public static IEnumerable<TrainingSession> GetInitSession()
        {
            var sessions = new List<TrainingSession>();

            for (var i = 0; i < 100; i++) 
            {
                var session = new TrainingSession()
                {
                    Name = $"Test Session {i+1}",
                    DateCreated = DateTime.UtcNow
                };

                var drill = new Drill()
                {
                    Description = "Defensive Skills",
                    Name = "Defense",
                    Duration = 20,
                    DateCreated = DateTime.UtcNow
                };
                session.AddDrill(drill);
                sessions.Add(session);
            }

            return sessions;
        }
    }
}
