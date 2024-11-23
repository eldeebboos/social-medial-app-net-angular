using API.Extentions;
using API.Middleware;

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

app.Run();
