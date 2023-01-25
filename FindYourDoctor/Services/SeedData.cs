using FindYourDoctor.Data;
using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FindYourDoctor.Services;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        await using var context = new FindYourDoctorDbContext(serviceProvider.GetRequiredService<DbContextOptions<FindYourDoctorDbContext>>());
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Account>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var userStore = serviceProvider.GetRequiredService<IUserStore<User>>();
        var emailStore = (IUserEmailStore<User>)userStore;
        var userRoleStore = (IUserRoleStore<User>)userStore;

        string[] roleNames = { "Unassigned", "Admin", "Patient", "Doctor" };

        foreach (var roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);

            if (role != null) continue;
            
            await roleManager.CreateAsync(new Account { Name = roleName });
        }

        var user = await userManager.FindByNameAsync("DefaultAdmin");

        if (user != null) return;
        
        user = Activator.CreateInstance<User>();
        await userStore.SetUserNameAsync(user, "DefaultAdmin", CancellationToken.None);
        await emailStore.SetEmailAsync(user, "admin@admin.com", CancellationToken.None);
        await userRoleStore.AddToRoleAsync(user, "Admin", CancellationToken.None);
        const string password = "Admin_1234";

        await userManager.CreateAsync(user, password);
    }
}