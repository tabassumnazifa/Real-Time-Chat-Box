using Amigoz.Data;
using Amigoz.Hubs;
using Amigoz.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --------------------------
// 1️⃣ Configure Database
// --------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add detailed DB exception page for development
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --------------------------
// 2️⃣ Configure Identity
// --------------------------
// Use custom AppUser instead of IdentityUser
builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// --------------------------
// 3️⃣ Add MVC & SignalR
// --------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// --------------------------
// 4️⃣ Build the app
// --------------------------
var app = builder.Build();

// --------------------------
// 5️⃣ Configure middleware pipeline
// --------------------------
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Use HSTS for production
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// --------------------------
// 6️⃣ Map routes
// --------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages (for Identity login/register)
app.MapRazorPages();

// Map SignalR hub
app.MapHub<ChatHub>("/chathub");

// --------------------------
// 7️⃣ Run the app
// --------------------------
app.Run();
