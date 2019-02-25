using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace TodoApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IHostingEnvironment environment)
        {
            Configuration = BuildConfigurations(environment).Build();
            Environment = environment;
        }

        public virtual void BuildLoggerConfigurations(IHostingEnvironment env)
        {
            string envSetting = Configuration["ASPNETCORE_ENVIRONMENT"];

            //var config = LogglyConfig.Instance;
            //config.CustomerToken = "47bf0a60-76fa-4925-a9ec-a7d784a045c6";
            //config.ApplicationName = Configuration["APPLICATION_NAME"] + "-" + envSetting;

            //config.Transport.EndpointHostname = "logs-01.loggly.com";
            //config.Transport.EndpointPort = 443;
            //config.Transport.LogTransport = LogTransport.Https;

            //var ct = new ApplicationNameTag { Formatter = "{0}" };
            //config.TagConfig.Tags.Add(ct);
            //Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(Configuration) // <= init logger
            //    .Enrich.WithExceptionDetails()
            //    .WriteTo.Loggly()
            //    .CreateLogger();
        }

        public virtual IConfigurationBuilder BuildConfigurations(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            return builder;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbconnectionstring = Configuration.GetConnectionString("SqlDbConnection");
            var db_servername = string.Empty;
            var db_username = string.Empty;
            var db_password = string.Empty;

            dbconnectionstring = dbconnectionstring.Replace("{db_servername}", Configuration["DB_SERVERNAME"] ?? "(localdb)\\mssqllocaldb");
            dbconnectionstring = dbconnectionstring.Replace("{db_username}", Configuration["DB_USERNAME"]);
            dbconnectionstring = dbconnectionstring.Replace("{db_password}", Configuration["DB_PASSWORD"]);

            //services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            services.AddDbContext<ProjectContext>(options => options.UseSqlServer(dbconnectionstring));
            services.BuildServiceProvider().GetService<ProjectContext>().Database.EnsureCreated();
            //services.BuildServiceProvider().GetService<ProjectContext>().Database.Migrate(); in case you want

            services.AddMvc();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }

            if (Environment.IsProduction() || Environment.IsStaging() || Environment.IsEnvironment("Staging_2"))
            {
                app.UseExceptionHandler("/Error");
                loggerFactory.AddAzureWebAppDiagnostics();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TODO API V1");
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Todos}/{action=Index}/{id?}");
            });
        }
    }
}