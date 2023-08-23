﻿using System;
using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace GhostUI.Extensions
{
    public static class ServiceCollectionExtensions


    {

        public static IServiceCollection AddCustomLocalization(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            //without country code "-xx" suffix => culture invariant
            var supportedCultures = new List<CultureInfo> { new("en"), new("fa") };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            return services;
        }

        public static IServiceCollection AddCorsConfig(this IServiceCollection services, string name)
        {
            
            
            services.AddCors(c => c.AddPolicy(name,
                options => options.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()));

            return services;
        }

        public static IServiceCollection AddResponseCompressionConfig(
            this IServiceCollection services,
            IConfiguration config,
            CompressionLevel compressionLvl = CompressionLevel.Fastest)
        {
            
            var enableForHttps = config.GetValue<bool>("Compression:EnableForHttps");
            var gzipMimeTypes = config.GetSection("Compression:MimeTypes").Get<string[]>();

            services.AddResponseCompression(options => {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = enableForHttps;
                options.MimeTypes = gzipMimeTypes ?? Array.Empty<string>();
            });

            services.Configure<BrotliCompressionProviderOptions>(options => options.Level = compressionLvl);
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = compressionLvl);

            return services;
        }
    }
}
