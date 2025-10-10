using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_1.Data.Services;
using Project_1.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Project_1.Hubs;
using Project_1.Services;

var builder = WebApplication.CreateBuilder(args);

// ----------------- Database -----------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ----------------- Identity -----------------
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>() // Add Roles
.AddEntityFrameworkStores<ApplicationDbContext>();

// ----------------- SignalR Service -----------------
builder.Services.AddSignalR();

// ----------------- Application Services -----------------
builder.Services.AddScoped<IListingsService, ListingsService>();
builder.Services.AddScoped<IBidsService, BidsService>();
builder.Services.AddScoped<ICommentsService, CommentsService>();

builder.Services.AddHostedService<BidExpirationService>();
builder.Services.AddSingleton<IEmailSender, NoEmailSender>();

builder.Services.AddControllersWithViews();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ----------------- Authorization -----------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// ----------------- Seed Admin Role & User -----------------
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Create Admin role if it doesn't exist
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Create Admin user if it doesn't exist
    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, "Admin@123"); // Password
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

// ----------------- Middleware -----------------
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ----------------- SignalR Hub Mapping -----------------
app.MapHub<BidHub>("/bidHub");

// ----------------- Routes -----------------
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Admin}/{action=Dashboard}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();

// ----------------- Fake Email Sender -----------------
public class NoEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;
    }
}

// ----------------- Hub -----------------
namespace Project_1.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    public class BidHub : Hub
    {
        public async Task UpdateHighestBid(int listingId, string newHighestBidAmount)
        {
            await Clients.Group($"listing-{listingId}")
                .SendAsync("ReceiveNewHighestBid", newHighestBidAmount);
        }

        public async Task JoinListingGroup(int listingId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"listing-{listingId}");
        }

        public async Task NotifyAuctionEnded(int listingId, string winner, decimal finalPrice)
        {
            await Clients.Group($"listing-{listingId}")
                .SendAsync("AuctionEnded", new { listingId, winner, finalPrice });
        }

        public async Task AdminBroadcast(string message)
        {
            await Clients.All.SendAsync("ReceiveAdminMessage", message);
        }
    }
}
