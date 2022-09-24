using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Core.Authorization;
using WebApi.Database.Models;
using WebApi.Services;

namespace WebApi
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
            //����� �� ������� �������� ��������� ����������� � ������ � ��������� ��� CurrencyService ��� ������
            services.AddHostedService<CurrencyService>();
            services.AddMemoryCache();
            //

            services.AddControllers();

            string connection = Configuration.GetConnectionString("default");
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(connection);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
                c.MapType<decimal>(() => new OpenApiSchema { Type = "number", Format = "decimal" });
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,  // ��������, ����� �� �������������� �������� ��� ��������� ������
                        ValidIssuer = AuthOptions.ISSUER, // ������, �������������� ��������
                        ValidateAudience = true,  // ����� �� �������������� ����������� ������
                        ValidAudience = AuthOptions.AUDIENCE, // ��������� ����������� ������
                        ValidateLifetime = true, //����� �� �������������� ����� �������������
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),  // ��������� ����� ������������
                        ValidateIssuerSigningKey = true, // ��������� ����� ������������
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStaticFiles();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
            }

            //app.UseHttpsRedirection();

            app.UseAuthentication();
           

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
