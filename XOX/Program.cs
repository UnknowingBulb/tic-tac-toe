using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;

namespace XOX
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static WebApplicationBuilder CreateHostBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
   
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromHours(20);
                    options.SlidingExpiration = true;
                    options.AccessDeniedPath = "/Forbidden/";
                });

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            return builder;
        }
    }
}
