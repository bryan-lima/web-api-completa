using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection WebApiConfig(this IServiceCollection services)
        {
            services.AddControllers();

            services.AddApiVersioning(options => 
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(majorVersion: 1, minorVersion: 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options => 
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(name: "Development",
                                  configurePolicy: builder => builder.AllowAnyOrigin()
                                                                     .AllowAnyMethod()
                                                                     .AllowAnyHeader());

                //options.AddDefaultPolicy(builder => builder.AllowAnyOrigin()
                //                                           .AllowAnyMethod()
                //                                           .AllowAnyHeader()
                //                                           .AllowCredentials());

                options.AddPolicy(name: "Production",
                                  configurePolicy: builder => builder.WithMethods("GET")
                                                                     .WithOrigins("http://desenvolvedor.io")
                                                                     .SetIsOriginAllowedToAllowWildcardSubdomains()
                                                                     //.WithHeaders(HeaderNames.ContentType, "x-custom-header")
                                                                     .AllowAnyHeader());
            });

            return services;
        }

        public static IApplicationBuilder UseMvcConfiguration(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            return app;
        }
    }
}
