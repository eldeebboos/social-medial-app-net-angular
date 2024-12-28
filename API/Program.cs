using API.Data;
using API.Entities;
using API.Extentions;
using API.Middleware;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Moved to Extentions/ ApplicationServiceExtentions Class
//and use the following line instead
//the extended function inside the class
builder.Services.AddApplicationServices(builder.Configuration);
//also use the other extention class for identity
builder.Services.AddIdentityService(builder.Configuration);

var app = builder.Build();

//Using custome meddileware to handle the exception
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(
    x =>
    x.AllowAnyHeader()
    .AllowAnyMethod()
    //to allow authentication at level of websocket
    .AllowCredentials()
    // .AllowAnyOrigin()
    .WithOrigins("http://localhost:4200", "https://localhost:4200")
    );
app.UseAuthentication();
app.UseAuthorization();

//Use the wwwroot folder
//for publishing the angular application side by side with .net dlls
app.UseDefaultFiles();
app.UseStaticFiles();
//Support routing of angular/fix the routing issue in angular/pass the routng responisbility to angular
app.MapFallbackToController("Index", "Fallback");

app.MapControllers();

//Add hub to use in SignalR
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

//seed data
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]");
    // await context.Database.ExecuteSqlRawAsync("DELETE FROM \"Connections\"");
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception e)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(e, "An error occured during the migration");
}

app.Run();
