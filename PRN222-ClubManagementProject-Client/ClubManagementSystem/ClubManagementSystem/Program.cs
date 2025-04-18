using System;
using BussinessObjects.Models;
using ClubManagementSystem.Controllers.Common;
using ClubManagementSystem.Controllers.Filter;
using ClubManagementSystem.Controllers.SignalR;
using ClubManagementSystem.Controllers.Worker;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repositories.Implementation;
using Repositories.Interface;
using Services.Implementation;
using Services.Interface;

var builder = WebApplication.CreateBuilder(args);

string keyVaultUri = builder.Configuration["AzureKeyVault:VaultUri"];

//var clientId = builder.Configuration["ClientId"];
//var clientSecret = builder.Configuration["ClientSecret"];

builder.Configuration.AddUserSecrets<Program>();
//string clientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
//string clientId = builder.Configuration["GoogleAuth:ClientId"];
string clientId = "439699209301-9iqj2l29punn6vaemn5k0tjr636b3gu5.apps.googleusercontent.com";
string clientSecret = "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";
// Add services to the container.
builder.Services.AddControllersWithViews();

//Add session
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//Add Repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IConnectionRepository, ConnectionRepository>();
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddScoped<SignalRSender>();


//Add Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IImageHelperService, ImageHelperService>();
builder.Services.AddScoped<IConnectionService, ConnectionService>();


//worker
builder.Services.AddHostedService<StatusUpdatingWorker>();

//filter
//builder.Services.AddScoped<ClubAuthorization>();

//signalR
builder.Services.AddSignalR();
builder.Services.AddScoped<SignalRSender>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://yourfrontend.com") // Change to your frontend URL
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

//Common
builder.Services.AddScoped<Week>();


// Add DbContext
builder.Services.AddDbContext<FptclubsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";  
    options.AccessDeniedPath = "/Account/AccessDenied"; 
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = clientId;
    options.ClientSecret = clientSecret;
    options.ClaimActions.MapJsonKey("urn:google:picture","picture","url");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors();
app.MapHub<ServerHub>("/serverHub");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

