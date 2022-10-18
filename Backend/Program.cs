using Backend.Data;
using Backend.Extensions;
using Backend.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddDependencyInjections();

builder.Services.AddControllers();
builder.Services.AddCors(setup =>
    {
        setup.AddPolicy("AngularLocalPolicy",
        config => { config.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod(); });
    });

builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

// app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));
app.UseCors("AngularLocalPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<Backend.Data.DataContext>();
    // var userManager = services.GetRequiredService<UserManager<AppUser>>();
    // var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(context); //userManager, roleManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

await app.RunAsync();

// app.Run();
