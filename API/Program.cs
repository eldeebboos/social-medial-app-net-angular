using API.Data;
using API.Extentions;
using API.Middleware;
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
    // .AllowAnyOrigin()
    .WithOrigins("http://localhost:4200", "https://localhost:4200")
    );
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//seed data
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(context);
}
catch (Exception e)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(e, "An error occured during the migration");
}

app.Run();
