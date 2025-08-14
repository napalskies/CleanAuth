using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        string connectionString = config.GetConnectionString("DefaultConnection");

        if(string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Connection string 'DefaultConnection' is not configured.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString,
                            ServerVersion.AutoDetect(connectionString)));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserManager<UserManager<ApplicationUser>>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddRoleManager<RoleManager<IdentityRole>>();

        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
