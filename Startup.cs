﻿using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SampleApp.Data;
using SampleApp.Filters;
using Microsoft.Extensions.FileProviders;

namespace SampleApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region snippet_AddMvc
            services.AddMvc()
                .AddRazorPagesOptions(options =>
                    {
                        options.Conventions
                            .AddPageApplicationModelConvention("/StreamedSingleFileUpload", 
                                model =>
                                {
                                    model.Filters.Add(
                                        new GenerateAntiforgeryTokenCookieAttribute());
                                    model.Filters.Add(
                                        new DisableFormValueModelBindingAttribute());
                                });
                    })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            #endregion

            #region snippet_FileProvider
            // To list physical files from a path provided by configuration:
            var physicalProvider = new PhysicalFileProvider(
                Configuration.GetValue<string>("StoredFilesPath"));

            // To list physical files in the temporary files folder, use:
            //var physicalProvider = new PhysicalFileProvider(Path.GetTempPath());

            services.AddSingleton<IFileProvider>(physicalProvider);
            #endregion

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ConnectionString")));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}