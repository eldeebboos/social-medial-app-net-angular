using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(DataContext context)
    {
        if (await context.Users.AnyAsync()) return;

        var userData = await File.ReadAllTextAsync("Data/UserDataSeed.json");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        if (users == null) return;

        foreach (var user in users)
        {
            user.UserName = user.UserName.ToLower();

            var hmac = new HMACSHA512();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("P@ssW0rd$"));
            user.PasswordSalt = hmac.Key;

            context.Users.Add(user);
        }

        await context.SaveChangesAsync();
    }
}
