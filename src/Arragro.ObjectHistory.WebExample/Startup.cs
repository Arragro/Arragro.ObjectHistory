using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arragro.Core.EntityFrameworkCore.Extensions;
using Arragro.Core.HostedServices;
using Arragro.ObjectHistory.Client.Extensions;
using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Models;
using Arragro.ObjectHistory.HostedService;
using Arragro.ObjectHistory.Web;
using Arragro.ObjectHistory.WebExample.Core.Entities;
using Arragro.ObjectHistory.WebExample.Core.Interfaces;
using Arragro.ObjectHistory.WebExample.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

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
                optionsBuilder => optionsBuilder.UseInMemoryDatabase("InMemoryDb"));
                //optionsBuilder => optionsBuilder.UseSqlServer("data source=localhost;initial catalog=ArragroObjectTracker;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework;", sqlOptions => sqlOptions.EnableRetryOnFailure(3)));
            
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            ConfigureObjectHistoryAuthenticationAndPolicies(services);
            var objectHistorySettings = new ObjectHistorySettings();
            Configuration.GetSection("ObjectHistorySettings").Bind(objectHistorySettings);
            services.AddArragroObjectHistoryClient<ObjectLogsSecurityAttribute>(objectHistorySettings);

            services.AddMvc()
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson();

            services
                .AddScoped<ObjectHistoryProcessor>()
                .AddQueueJob<ObjectProcessorHostedService>(
                    objectHistorySettings.AzureStorageConnectionString,
                    objectHistorySettings.ObjectQueueName
                 );

            services.AddScoped<ITrainingSessionRepository, EFTrainingSessionRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //CreateAndMigrateDatabase(app.ApplicationServices);

                // app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                // {
                //     HotModuleReplacement = true
                // });

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
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default-spa",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapFallbackToController("SpaIndex", "Session");
            });;
        }

        public void CreateAndMigrateDatabase(
            IServiceProvider serviceProvider)
        {
            var demoDbContext = serviceProvider.GetService<DemoDbContext>();

            var demoDbExists = demoDbContext.Exists();

            if (!demoDbExists || (!demoDbContext.AllMigrationsApplied()))
            {
                demoDbContext.Database.Migrate();
                if (!demoDbContext.TrainingSessions.Any())
                {
                    InitializeDatabaseAsync(serviceProvider).Wait();
                }
            }
        }

        public async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            var repository = serviceProvider.GetService<ITrainingSessionRepository>();
            var sessionList = await repository.ListAsync();
            var sessions = GetInitSession();

            using (var scope = serviceProvider.CreateScope())
            {
                repository = scope.ServiceProvider.GetService<ITrainingSessionRepository>();
                if (!sessionList.Any())
                {
                    foreach (var session in sessions)
                    {
                        await repository.AddAsync(session);
                    }
                }
            }

            if (!sessionList.Any())
            {
                for (var i = 0; i < 100; i++)
                {
                    repository = serviceProvider.GetService<ITrainingSessionRepository>();
                    var session = await repository.GetByIdAsync(1);
                    var mod = session.Clone();
                    mod.AddDrill(new Drill
                    {
                        Name = $"Test {i + 1}",
                        Description = $"Description {i + 1}",
                        SkillLevel = Difficulty.Beginner
                    });
                    using (var scope = serviceProvider.CreateScope())
                    {
                        repository = scope.ServiceProvider.GetService<ITrainingSessionRepository>();
                        await repository.UpdateAsync(mod, session);
                        session = await repository.GetByIdAsync(1);
                    }
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
    public static class CloningService
    {
        public static T Clone<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            var serializeSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source, serializeSettings), deserializeSettings);
        }
    }
}
