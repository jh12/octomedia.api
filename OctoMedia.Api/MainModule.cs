using System.Diagnostics;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace OctoMedia.Api
{
    public class MainModule : Module
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public MainModule(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterLogger(builder);
        }

        private void RegisterLogger(ContainerBuilder builder)
        {
            LoggerConfiguration configuration = new LoggerConfiguration();

            configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

            configuration
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithProperty("AssemblyName", ThisAssembly.GetName().Name);

            if (Debugger.IsAttached || _environment.IsDevelopment())
                configuration
                    .WriteTo.Console();
            else
                configuration
                    .WriteTo.Seq(_configuration["LOGGING:SEQ:URL"], apiKey: _configuration["LOGGING:SEQ:APP_Token"]);

            Logger logger = configuration.CreateLogger();
            Log.Logger = logger;

            builder.Register(f => logger).As<ILogger>().SingleInstance();
        }
    }
}