using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HttpLoadTester.SignalR;
using HttpLoadTester.Services;
using HttpLoadTester.Services.Scenarios;
using System.Linq;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace HttpLoadTester
{
    public class Startup
    {
        
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
            });

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<ServiceActions>();

            
            AddDependencyInjectionForTypes(new[] { typeof(ITest) }, services);

            services.AddTransient<ServiceRunner>();

            var config = new TestConfiguration () 
                    { BaseUrl = Configuration["BaseUrl"]
                      , ConcurrentUsersPerTest = int.Parse(Configuration["ConcurrentUsersPerTest"])
                      , UserWaitSeconds = int.Parse(Configuration["UserWaitSeconds"]) 
                      , EpisodeIDs = Configuration["EpisodeIDs"].Split(',').Select(s => int.Parse(s)).ToArray()
                      , CookieUserName = Configuration["CookieUserName"]
            };

            services.AddSingleton<TestConfiguration>(config);
        }

        private void AddDependencyInjectionForTypes (IEnumerable<Type> types, IServiceCollection services)
        {
            var assemblies = GetReferencingAssemblies("HttpLoadTester");

            foreach (var type in types)
            {
                foreach (var test in assemblies.SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p) && p != type))
                {
                    var name = test.Name;
                    services.AddTransient(typeof(ITest), test);
                }
            }
        }
        private static IEnumerable<Assembly> GetReferencingAssemblies(string assemblyName)
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (IsCandidateLibrary(library, assemblyName))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
            }
            return assemblies;
        }

        private static bool IsCandidateLibrary(RuntimeLibrary library, string assemblyName)
        {
            return library.Name == (assemblyName)
                || library.Dependencies.Any(d => d.Name.StartsWith(assemblyName));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseWebSockets();
            app.UseSignalR<RawConnection>("/raw-connection");
            app.UseSignalR();
        }
    }
}
