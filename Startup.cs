using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.EntityFrameworkCore;
using OnlineStore.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OnlineStore.Controllers;
using OnlineStore.helpers;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace OnlineStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var connString = Configuration.GetConnectionString("connString");

            var server = Environment.GetEnvironmentVariable("server");

            connString = connString.Replace("{{server}}", server).
                Replace("{{username}}", Environment.GetEnvironmentVariable("dbusername")).
                Replace("{{database}}", Environment.GetEnvironmentVariable("dbname")).
                Replace("{{password}}", Environment.GetEnvironmentVariable("dbpassword"));

            services.AddDbContext<OnlineStorageContext>(opt => opt.UseSqlServer(connString, o => o.CommandTimeout(180)));

            var jwtTokenConfig = Configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>();

            services.AddSingleton(jwtTokenConfig);
            services.AddSingleton<IJwtAuthManager, JwtAuthManager>();

            services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowAllHeaders",
                    builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            services.AddControllers().AddNewtonsoftJson();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseCors("AllowAllHeaders");

            app.UseHttpsRedirection();

            app.UseStatusCodePages();

            app.UseRouting();

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
