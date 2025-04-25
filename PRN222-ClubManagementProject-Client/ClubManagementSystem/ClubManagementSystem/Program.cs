using System;
using BussinessObjects.Models;
using CloudinaryDotNet;
using ClubManagementSystem.Controllers.Common;
using ClubManagementSystem.Controllers.Filter;
using ClubManagementSystem.Controllers.SignalR;
using ClubManagementSystem.Controllers.Worker;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Repositories.Implementation;
using Repositories.Interface;
using Services.Implementation;
using Services.Interface;
using ClubManagementSystem.Services;
using ClubManagementSystem.Utilities;
using Serilog;
using ClubManagementSystem.Controllers.Hubs;

var builder = WebApplication.CreateBuilder(args);

string keyVaultUri = builder.Configuration["AzureKeyVault:VaultUri"];

//var clientId = builder.Configuration["ClientId"];
//var clientSecret = builder.Configuration["ClientSecret"];

builder.Configuration.AddUserSecrets<Program>();
//string clientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
//string clientId = builder.Configuration["GoogleAuth:ClientId"];

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
builder.Services.AddScoped<IClubRequestRepository, ClubRequestRepository>();
builder.Services.AddScoped<IClubRepository, ClubRepository>();
builder.Services.AddScoped<IConnectionRepository, ConnectionRepository>();
builder.Services.AddScoped<IJoinRequestRepository, JoinRequestRepository>();
builder.Services.AddScoped<IClubMemberRepository, ClubMemberRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IClubMemberRepository, ClubMemberRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IPostReactionRepository, PostReactionRepository>();
builder.Services.AddScoped<IClubTaskRepository, ClubTaskRepository>();
builder.Services.AddScoped<ITaskAssignmentRepository, TaskAssignmentRepository>();
builder.Services.AddScoped<IMembershipFeeRepository, MembershipFeeRepository>();
builder.Services.AddScoped<IClubStatsRepository, ClubStatsRepository>();

//Add Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IClubRequestService, ClubRequestService>();
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<IImageHelperService, ImageHelperService>();
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddScoped<IJoinRequestService, JoinRequestService>();
builder.Services.AddScoped<IClubMemberService, ClubMemberService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IClubMemberService, ClubMemberService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IPostReactionService, PostReactionService>();
builder.Services.AddScoped<IClubTaskService, ClubTaskService>();
builder.Services.AddScoped<ITaskAssignmentService, TaskAssignmentService>();
builder.Services.AddScoped<IMembershipFeeService, MembershipFeeService>();
builder.Services.AddScoped<IClubStatsService, ClubStatsService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();

// Cấu hình Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .MinimumLevel.Information() // Ghi log từ mức Information trở lên
        .WriteTo.Console() // Vẫn hiển thị trên console
        .WriteTo.File(
            path: "logs/email-worker-.log", // Đường dẫn file log, thêm ngày vào tên file
            rollingInterval: RollingInterval.Day, // Tạo file mới mỗi ngày
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}" // Định dạng log
        );
});

var cloudName = builder.Configuration["Cloudinary:CloudName"];
var apiKey = builder.Configuration["Cloudinary:ApiKey"];
var apiSecret = builder.Configuration["Cloudinary:ApiSecret"];

var account = new Account(cloudName, apiKey, apiSecret);
var cloudinary = new Cloudinary(account);

builder.Services.AddSingleton(cloudinary);


//worker
builder.Services.AddHostedService<StatusUpdatingWorker>();
// Worker Service
builder.Services.AddSingleton<IQueueService, InMemoryQueueService>();
builder.Services.AddHostedService<EmailWorker>();
builder.Services.AddHostedService<MembershipFeeReminderWorker>();
//filter
builder.Services.AddScoped<ClubAuthorization>();

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


builder.Services.AddScoped<IVnPayService, VnPayService>();

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

app.MapHub<NotificationHub>("/notificationHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

