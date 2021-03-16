using HotelListing.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing
{
    // this class is set up because we don't want to overload our startup class
    public static class ServiceExtenstions
    {
        /* "this" keyword means that this method is an extension method. So we bind
           the method and "attach" it to the services in the ConfigureServices method
           in the Startup class */
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<ApiUser>(q => q.User.RequireUniqueEmail = true);

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);

            // Where we store users' info and what tokens we have (?)
            builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
        }

        // JWT setup
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration Configuration)
        {
            var jwtSettings = Configuration.GetSection("Jwt"); // appsettings.json
            // The rule is to not put the key in the app settings (!)

            // Bad solution. But I don't know how to properly work with my
            // env vars
            ////var key = Environment.GetEnvironmentVariable("KEY"); // env
            var key = "my_super_duper_secure_key";

            services.AddAuthentication(o =>
            {
                // When someone is trying to authenticate, check for the JWT token in 
                // their request header
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    // ValidateAudience = false is VERY important. Spend 30 minutes debugging
                    // this crap
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
        }
    }
}
