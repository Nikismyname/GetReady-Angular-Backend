namespace GetReady.Web
{
    using GetReady.Data;
    using GetReady.Services.Contracts;
    using GetReady.Services.Implementations;
    using GetReady.Services.Mapping;
    using GetReady.Services.Models.QuestionModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using GetReady.Data.Models.QuestionModels;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            AutoMapperConfig.RegisterMappings(
                typeof(CopyQuestions).Assembly,
                typeof(GlobalQuestionPackage).Assembly);

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ISeederService, SeederService>();
            services.AddTransient<IQuestionSheetService, QuestionSheetService>();
            services.AddTransient<IQuestionService, QuestionService>();
            services.AddTransient<IJwtService, JwtService>();

            services.AddDbContext<GetReadyDbContext>
                (o => o.UseSqlServer(Configuration.GetConnectionString("GetReady")));

            services.AddCors();

            var key = Convert.FromBase64String(Configuration["Jwt:SigningSecret"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //https://github.com/aspnet/AspNetCore/issues/6166
            app.Use(async (ctx, next) =>
            {
                await next();
                if (ctx.Response.StatusCode == 204)
                {
                    ctx.Response.ContentLength = 0;
                }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:3000", "http://localhost:4200");
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
            });

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseMvc();
        }
    }
}
