using CapstonePRS.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(x =>
{
    string ConnectionKey = "Dev";
#if RELEASE
    ConnectionKey = "Prod";
#endif
    x.UseSqlServer(builder.Configuration.GetConnectionString(ConnectionKey));
});


var app = builder.Build();

    //may want to revisit and restrict
app.UseCors(x =>
{
    x.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod();
});

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

   //statements to auto update database when run, optional.
using var scope = app.Services
                    .GetRequiredService<IServiceScopeFactory>()
                    .CreateScope();
scope.ServiceProvider
        .GetService<AppDbContext>()!
        .Database.Migrate();

app.Run();
