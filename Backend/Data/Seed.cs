using System.Text.Json;
using Backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");

        var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

        if(users == null) return;

        foreach (var item in users)
        {
            item.UserName = item.UserName.ToLower();

            await userManager.CreateAsync(item, "Pa$$w0rd");
        }
    }
}
