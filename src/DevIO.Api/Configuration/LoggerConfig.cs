using DevIO.Api.Extensions;
using Elmah.Io.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddElmahIo(options =>
            {
                options.ApiKey = "ab4390cb8f0a44f29ed0d05bfe79a0b2";
                options.LogId = new Guid("e2581f7f-4466-463f-82ea-924d73aa4e77");
            });

            //services.AddLogging(builder => 
            //{
            //    builder.AddElmahIo(o =>
            //    {
            //        o.ApiKey = "ab4390cb8f0a44f29ed0d05bfe79a0b2";
            //        o.LogId = new Guid("e2581f7f-4466-463f-82ea-924d73aa4e77");
            //    });

            //    builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
            //});

            services.AddHealthChecks()
                    .AddElmahIoPublisher(options =>
                    {
                        options.ApiKey = "ab4390cb8f0a44f29ed0d05bfe79a0b2";
                        options.LogId = new Guid("e2581f7f-4466-463f-82ea-924d73aa4e77");
                        options.HeartbeatId = "API Fornecedores";
                    })
                    .AddCheck("Produtos", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultConnection")))
                    .AddSqlServer(configuration.GetConnectionString("DefaultConnection"), name: "BancoSQL");

            services.AddHealthChecksUI()
                    .AddSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));

            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();

            return app;
        }
    }
}
