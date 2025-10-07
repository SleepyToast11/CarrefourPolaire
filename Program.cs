using CarrefourPolaire.Data;
using CarrefourPolaire.Models;
using CarrefourPolaire.Services;
using CarrefourPolaire.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("Smtp"));

// builder.Services.AddSingleton(resolver =>
//     resolver.GetRequiredService<IOptions<SmtpSettings>>().Value);

builder.Services.AddScoped<IEmailService>(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<SmtpSettings>>().Value;
        return new EmailService(settings);
    }
    );

builder.Services.AddScoped<IInviteTokenService, InviteTokenService>();

// Add services to the container.
builder.Services.AddRazorPages();
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Connection: {builder.Configuration.GetConnectionString("DefaultConnection")}");

// Register DbContext with connection string
builder.Services.AddDbContext<EventContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("EmailLink")
    .AddCookie("EmailLink", options =>
    {
        options.LoginPath = "/Login";  // redirect if unauthenticated
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // session lifetime
        options.SlidingExpiration = true;
    });


var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<EventContext>();
db.Database.Migrate();


app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
