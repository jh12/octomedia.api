using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OctoMedia.Api.Common.Options;
using OctoMedia.Api.DataAccess.Mssql;
using OctoMedia.Api.Middleware;
using OctoMedia.Api.Modules;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace OctoMedia.Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.Filters.Add<HttpResponseExceptionFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            services.AddSwaggerGen();

            services.AddHttpClient();
            
            services.Configure<LoggingOptions>(Configuration.GetSection(LoggingOptions.Key));
            services.Configure<MssqlOptions>(Configuration.GetSection(MssqlOptions.Key));
            services.Configure<ProxyOptions>(Configuration.GetSection(ProxyOptions.Key));
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            LoggingOptions loggingOptions = new LoggingOptions();
            Configuration.GetSection(LoggingOptions.Key).Bind(loggingOptions);
            
            builder.RegisterModule(new MainModule(_hostEnvironment, loggingOptions));
            builder.RegisterModule<MssqlModule>();

#if DEBUG
            builder.RegisterModule(new FileModule(true));
#else
            builder.RegisterModule(new FileModule(false));
#endif
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "OctoMedia API V1");
                c.RoutePrefix = string.Empty;
                c.DocExpansion(DocExpansion.None);
                c.DisplayRequestDuration();
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
