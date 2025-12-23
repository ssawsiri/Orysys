using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<OrysysLoanApplication.DataAccess.DataAccessLoanApplication>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login";
        options.Cookie.Name = "OrysysLoanApp.Auth";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
        options.SlidingExpiration = true;
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

try
{
    var schemeProvider = app.Services.GetRequiredService<IAuthenticationSchemeProvider>();
    var schemes = schemeProvider.GetAllSchemesAsync().GetAwaiter().GetResult();
    var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("StartupDiagnostics");

    var schemesList = schemes?.ToList();
    if (schemesList is null || schemesList.Count == 0)
    {
        logger.LogWarning("No authentication schemes registered.");
    }
    else
    {
        foreach (var s in schemesList)
        {
            logger.LogInformation("Auth scheme registered: {SchemeName}, HandlerType: {HandlerType}", s.Name, s.HandlerType?.FullName ?? "<null>");
        }
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("StartupDiagnostics");
    logger.LogError(ex, "Failed to enumerate authentication schemes.");
}

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
