using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infra.Repository.Mongo;
using Infra.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UI.Web.Services.Authorization;

namespace UI.Web
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

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IUsuarioAppService, UsuarioAppService>();
            services.AddScoped<IAlunoRepository, AlunoRepository>();
            services.AddScoped<IAlunoAppService, AlunoAppService>();
            services.AddScoped<IProfessorRepository, ProfessorRepository>();
            services.AddScoped<IProfessorAppService, ProfessorAppService>();
            services.AddScoped<IDisciplinaRepository, DisciplinaRepository>();
            services.AddScoped<IDisciplinaAppService, DisciplinaAppService>();

            services.AddScoped<IHashing, BCryptHashing>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            services.AddAuthorization(config => 
            {

                config.AddPolicy("UserPolicy", policyBuilder => 
                {
                    
                    policyBuilder.UserRequireCustomClaim(ClaimTypes.Name);
                    
                });

            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();

            services.AddControllersWithViews();

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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
