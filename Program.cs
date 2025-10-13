using System.Globalization;
using CarrefourPolaire.Data;
using CarrefourPolaire.Models;
using CarrefourPolaire.Models.Configs;
using CarrefourPolaire.Services;
using CarrefourPolaire.Services.Interfaces;
using Microsoft.AspNetCore.Localization;
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


// Add services to the container.
builder.Services.AddRazorPages()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();
    ;

builder.Services.AddLocalization();
builder.Services.AddHttpContextAccessor();

// Register DbContext with connection string
builder.Services.AddDbContext<EventContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("EmailLink")
    .AddCookie("EmailLink", options =>
    {
        options.LoginPath = "/Login";  // redirect if unauthenticated
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(14); // session lifetime
        options.SlidingExpiration = true;
    });

var names = typeof(Program).Assembly.GetManifestResourceNames();
foreach (var name in names)
{
    Console.WriteLine("Embedded resource: " + name);
}
var app = builder.Build();

var supportedCultures = new[] { "fr", "en" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("fr")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

// Optional: disable automatic Accept-Language fallback if you want FR to always win
localizationOptions.RequestCultureProviders = new List<IRequestCultureProvider>
{
    new QueryStringRequestCultureProvider(),  // ?culture=fr
    new CookieRequestCultureProvider(),        // culture saved in cookie
    // comment out the AcceptLanguageProvider to avoid browser locale overriding
    new AcceptLanguageHeaderRequestCultureProvider()
};

app.UseRequestLocalization(localizationOptions);

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

app.MapControllers();
app.MapRazorPages();
app.MapFallbackToPage("/Index"); 

app.Run();
