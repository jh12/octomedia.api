using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OctoMedia.Api.Common.Options;
using OctoMedia.Api.DataAccess.FileSystem.Configuration;
using OctoMedia.Api.DataAccess.Mssql;
using OctoMedia.Api.DataAccess.Mssql.Configuration;
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
            
            services.Configure<ProxyOptions>(Configuration.GetSection(ProxyOptions.Key));
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            RegisterConfigurationSections(builder);

            builder.RegisterModule(new MainModule(_hostEnvironment, Configuration));
            builder.RegisterModule<MssqlModule>();

#if DEBUG
            builder.RegisterModule(new FileModule(true));
#else
            builder.RegisterModule(new FileModule(false));
#endif
        }

        private void RegisterConfigurationSections(ContainerBuilder builder)
        {
            builder.Register(f => Configuration.GetSection("Mssql").Get<MssqlConfiguration>()).As<MssqlConfiguration>();
            builder.Register(f => Configuration.GetSection("FileSystem").Get<FileSystemConfiguration>()).As<FileSystemConfiguration>();
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
