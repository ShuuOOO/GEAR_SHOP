using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using System;
using TL4_SHOP.Services.Auth;

namespace TL4_SHOP.Extensions
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAppAuth(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(opt =>
                    {
                        opt.LoginPath = "/Account/Login";
                        opt.AccessDeniedPath = "/Account/AccessDenied";
                        opt.SlidingExpiration = true;
                        opt.ExpireTimeSpan = TimeSpan.FromHours(8);
                    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", p => p.RequireRole(AppRoles.Admin));
                options.AddPolicy("AnyStaff", p => p.RequireRole(AppRoles.AllStaff));
                options.AddPolicy("AdminOrProductManager", p => p.RequireRole(AppRoles.Admin, AppRoles.ProductManager));
                options.AddPolicy("AdminOrOrderManager", p => p.RequireRole(AppRoles.Admin, AppRoles.OrderManager));
                options.AddPolicy("AdminOrHRManager", p => p.RequireRole(AppRoles.Admin, AppRoles.HRManager));
                options.AddPolicy("AdminOrCustomerCare", p => p.RequireRole(AppRoles.Admin, AppRoles.CustomerCare));
            });

            services.AddScoped<IRoleResolver, RoleResolver>();
            return services;
        }
    }
}
