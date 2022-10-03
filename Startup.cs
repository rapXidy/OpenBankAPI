using BankAPI.DAL;
using BankAPI.Services;
using BankAPI.Services.Implementations;
using BankAPI.Services.Interfaces;
using BankAPI.Util;
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
using System.Text;
using System.Threading.Tasks;

namespace BankAPI
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
            services.AddDbContext<BankAPIDBContext>(x => x.UseSqlServer(
                Configuration.GetConnectionString("BankAPIConnection")));

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Type = SecuritySchemeType.Http,
                    Description = "Bearer Authentication using JWT scheme"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference= new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearer"
                            }
                        },
                        new string[]{}

                    }
                });
                c.SwaggerDoc("v1", new OpenApiInfo 
                {
                    Title = "OpenBankAPI", 
                    Version = "v1",
                    Description = "Banks API Documentation. lol crazy enough to build a bank api",
                    TermsOfService = new Uri("https://www.github.com/rapxidy/"),
                    Contact = new OpenApiContact
                    {
                        Name = "Chidii.dll",
                        Email = "chidiebere.levi@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/chidiebere-levi-070907156/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Open Source banking API",
                        Url = new Uri("https://www.linkedin.com/in/chidiebere-levi-070907156/"),
                    }

                });
            });

            services.AddSingleton<IJWTManagerRepository, JWTManagerRepository>();
            
            services.AddAuthentication(authOpt =>
            {
                authOpt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOpt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(o =>
              {
                  o.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateAudience = false,
                      ValidateIssuer = false,
                      //ValidIssuer = Configuration["Jwt:Issuer"],
                      //ValidAudience = Configuration["Jwt:Audience"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                      ClockSkew = TimeSpan.Zero // remove delay of token when expire

                  };
              });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BankAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
