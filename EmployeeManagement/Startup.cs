using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));
            
            
            services.AddIdentity<ApplicationUser, IdentityRole>(Options =>
            {
                Options.Password.RequiredLength = 8;
                Options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
                Options.SignIn.RequireConfirmedEmail = true;
                Options.Lockout.MaxFailedAccessAttempts = 5;
                Options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);


            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation"); 

            
            
            services.AddMvc(option => { option.EnableEndpointRouting = false;
                var policy = new AuthorizationPolicyBuilder().
                                RequireAuthenticatedUser().Build();
                option.Filters.Add(new AuthorizeFilter());
                });

            
            
            services.Configure<DataProtectionTokenProviderOptions>(o =>
            o.TokenLifespan = TimeSpan.FromHours(5));

            
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o =>
                        o.TokenLifespan = TimeSpan.FromDays(5));

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "315593074188-hbhdbc8di2cmtq1vc0fdjluah4oiodca.apps.googleusercontent.com";
                    options.ClientSecret = "j-cqW8YYOqAMYzvybNjYRMS6"; 
                })
                .AddFacebook(options => {
                    options.AppId = "3632469393461032";
                    options.AppSecret = "7479ca2d991f2100667cc0dd84573114";
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy", 
                                    policy => policy.RequireClaim("Delete Role"));
                //options.AddPolicy("EditRolePolicy",
                //                    policy => policy.RequireClaim("Edit Role", "True"));

                //options.AddPolicy("EditRolePolicy",
                //                     policy => policy.RequireAssertion(context => 
                //                   context.User.IsInRole("Admin") &&
                //                   context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "True") ||
                //                    context.User.IsInRole("SuperAdmin")
                //                    ));

                //  options.InvokeHandlersAfterFailure = false;

                options.AddPolicy("EditRolePolicy",
                                     policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

                options.AddPolicy("AdminRolePolicy",
                                    policy => policy.RequireRole("Admin"));
            });

            services.AddScoped<IEmployeeRepository, SqlEmployeeRepository>();

            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
            services.AddSingleton<DataProtectionPurposeStrings>();

            

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

                // This is least used status code, as it its similar to the one which browser shows on 404.
                //app.UseStatusCodePages();

                // {} gets the actual status code like 404 etc
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }


            app.UseRouting();

            app.UseStaticFiles();

            app.UseAuthentication();

            //app.UseMvcWithDefaultRoute();

            app.UseMvc(Routes =>
            {
                Routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseMvc();




            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync(_config["MyKey"]);
            //    });
            //});
        }
    }
}
