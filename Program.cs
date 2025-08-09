using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Hubs;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<_4tlShopContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("4TL_SHOP"));
});
builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();

QuestPDF.Settings.License = LicenseType.Community;


builder.Services.AddSession();


// Configure services
ConfigureServices(builder.Services, builder.Configuration);

// Build the application
var app = builder.Build();

app.MapHub<ChatHub>("/chatHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
// seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<_4tlShopContext>();

    if (!context.TrangThaiDonHangs.Any())
    {
        context.TrangThaiDonHangs.AddRange(
            new TrangThaiDonHang { TrangThaiId = 1, TenTrangThai = "Chờ xác nhận" },
            new TrangThaiDonHang { TrangThaiId = 2, TenTrangThai = "Đã xác nhận" },
            new TrangThaiDonHang { TrangThaiId = 3, TenTrangThai = "Đang giao" },
            new TrangThaiDonHang { TrangThaiId = 4, TenTrangThai = "Giao thành công" },
            new TrangThaiDonHang { TrangThaiId = 5, TenTrangThai = "Đã hủy" }
        );
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline
ConfigurePipeline(app);

app.Run();

// Service configuration method
static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Core services
    services.AddControllersWithViews();
    services.AddHttpContextAccessor();

    // Session configuration
    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromDays(7); // giữ session 7 ngày
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

    // Database configuration
    services.AddDbContext<_4tlShopContext>(options =>
    {
        var connectionString = configuration.GetConnectionString("4TL_SHOP");
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
    });

    // Authentication configuration
    ConfigureAuthentication(services, configuration);

    // Authorization configuration
    ConfigureAuthorization(services);
}

// Authentication configuration method
static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
{
    services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = configuration["Authentication:Google:ClientId"] ?? "115282379706-...";
        googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? "GOCSPX-...";
        googleOptions.SaveTokens = true;
    })
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = configuration["Authentication:Facebook:AppId"] ?? "FACEBOOK_APP_ID";
        facebookOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"] ?? "FACEBOOK_APP_SECRET";
        facebookOptions.SaveTokens = true;
    });
}

// Authorization configuration method
static void ConfigureAuthorization(IServiceCollection services)
{
    services.AddAuthorization(options =>
    {
        // Admin policy
        options.AddPolicy("AdminOnly", policy =>
            policy.RequireRole("Admin"));

        // Customer policy
        options.AddPolicy("CustomerOnly", policy =>
            policy.RequireRole("Customer"));

        // Authenticated user policy
        options.AddPolicy("AuthenticatedUser", policy =>
            policy.RequireAuthenticatedUser());
    });
}

// Pipeline configuration method
static void ConfigurePipeline(WebApplication app)
{
    // Error handling
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    // Security headers
    app.UseHttpsRedirection();

    // Static files
    app.UseStaticFiles();

    // Routing
    app.UseRouting();

    // Session
    app.UseSession();

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSession();


    // Route mapping
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "admin",
        pattern: "admin/{controller=Dashboard}/{action=Index}/{id?}",
        defaults: new { area = "Admin" });
}
