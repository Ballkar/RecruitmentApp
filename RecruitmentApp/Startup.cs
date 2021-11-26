using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using RecruitmentApp.Entities;
using RecruitmentApp.Services;
using RecruitmentApp;
using RecruitmentApp.Authorization;
using RecruitmentApp.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using RecruitmentApp.Models;
using RecruitmentApp.Models.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RecruitmentApp
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
            var authenticationSettings = new AuthenticationSettings();

            Configuration.GetSection("Authentication").Bind(authenticationSettings);

            services.AddSingleton(authenticationSettings);
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });
            services.AddAuthorization(options =>
            {

            });
            

            services.AddControllers()
                .AddFluentValidation();

            services.AddAutoMapper(this.GetType().Assembly);

            //MyServiceHere
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, ExperienceOperationRequirementHandler>();

            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddDbContext<RecruitmentAppDbContext>();
            services.AddScoped<RecruitmentAppSeeder>();
            services.AddScoped<IErrorLogService, ErrorLogService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<ISeniorityService, SeniorityService>();
            services.AddScoped<IResumeService, ResumeService>();
            services.AddScoped<ISkillsService, SkillsService>();
            services.AddScoped<IExperienceService, ExperienceService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            //Validators
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
            services.AddScoped<IValidator<AddFeedbackDto>, AddFeedbackDtoValidator>();
            services.AddScoped<IValidator<CreateExperienceDto>, CreateExperienceDtoValidator>();
            services.AddScoped<IValidator<UpdateExperienceDto>, UpdateExperienceDtoValidator>();
            services.AddScoped<IValidator<CreateResumeDto>, CreateResumeDtoValidator>();
            services.AddScoped<IValidator<UpdateResumeDto>, UpdateResumeDtoValidator>();
            
            //
            services.AddScoped<RequestTimeMiddleware>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen();
            services.AddHttpClient();


            services.AddCors(options =>
            {
                options.AddPolicy("FrontEndClient", builder =>

                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowAnyOrigin()
                        .SetIsOriginAllowed((host) => true)
                    );
            });

            services.AddDbContext<RecruitmentAppDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RecruitmentAppSeeder seeder)
        {
            app.UseResponseCaching();
            app.UseStaticFiles();
            seeder.Seed();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseMiddleware<RequestTimeMiddleware>();

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

        }
    }
}
