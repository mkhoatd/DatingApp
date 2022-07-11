using System.Security.Cryptography;
using DatingAppAPI.Entities;
using DatingAppAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Data;

public class Seed
{
    public static async Task SeedUsers(DataContext context, UserService userService)
    {
        if (await context.Users.AnyAsync()) return;

        var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
        foreach (var user in users)
        {
            var FullUser = userService.CreateUser(user.UserName.ToLower(), "password");
            context.Users.Add(FullUser);
        }
        context.SaveChangesAsync();
    }
}