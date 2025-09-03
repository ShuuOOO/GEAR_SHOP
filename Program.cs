using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;
using TL4_SHOP.Hubs;
using QuestPDF.Infrastructure;
using TL4_SHOP.Extensions;       // AddAppAuth()
using TL4_SHOP.Services.Auth;   // IRoleResolver (nếu bạn có DI)

var builder = WebApplication.CreateBuilder(args);

// ===== Infra giữ nguyên =====
builder.Services.AddSignalR();
QuestPDF.Settings.License = LicenseType.Community;

// ===== Gọi hàm cấu hình dịch vụ =====
ConfigureServices(builder.Services, builder.Configuration);

// ===== Build app =====
var app = builder.Build();

// ===== Seed trạng thái đơn hàng (giữ nguyên) =====
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

// ===== Pipeline =====
ConfigurePipeline(app);

app.Run();


// ======================= Services =======================
static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // MVC + Filters (gom 1 chỗ, tránh lặp)
    services.AddControllersWithViews(o =>
    {
        o.Filters.Add<TL4_SHOP.Extensions.SqlErrorToMessageFilter>();
        o.Filters.Add<TL4_SHOP.Filters.NotifyPingFilter>();
        o.Filters.Add<TL4_SHOP.Filters.SetActorContextFilter>();
    });
    services.AddScoped<TL4_SHOP.Filters.SetActorContextFilter>();
    services.AddScoped<TL4_SHOP.Filters.NotifyPingFilter>();

    // HttpContext + Session
    services.AddHttpContextAccessor();
    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromDays(7);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

    // DbContext
    services.AddDbContext<_4tlShopContext>(options =>
    {
        var connectionString = configuration.GetConnectionString("4TL_SHOP");
        options.UseSqlServer(connectionString, sql =>
            sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null));
    });

    // ============ AUTH ============

    // 1) Cookie + default policies được AddAppAuth thiết lập sẵn
    services.AddAppAuth();

    // 2) NỐI THÊM provider ngoài — KHÔNG AddCookie lần nữa
    services.AddAuthentication()
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

    // 3) (Tuỳ dự án) Nếu bạn muốn bổ sung chính sách riêng ngoài AddAppAuth:
    ConfigureAuthorization(services);

    // DI cho resolver role (nếu dùng)
    services.AddScoped<IRoleResolver, RoleResolver>();
}

// =================== Authorization (tuỳ chọn bổ sung) ===================
static void ConfigureAuthorization(IServiceCollection services)
{
    services.AddAuthorization(options =>
    {
        // Ví dụ policy cơ bản; nếu AddAppAuth đã có, đây là phần bổ sung thêm.
        options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));

        // Khách hàng
        options.AddPolicy("CustomerOnly", p => p.RequireRole("Customer", "KhachHang"));

        // Đã đăng nhập
        options.AddPolicy("AuthenticatedUser", p => p.RequireAuthenticatedUser());
    });
}

// ======================= Pipeline =======================
static void ConfigurePipeline(WebApplication app)
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseSession();          // session
    app.UseAuthentication();   // auth
    app.UseAuthorization();

    // Areas
    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

    // Default route
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // SignalR
    app.MapHub<ChatHub>("/chatHub");
    app.MapHub<TL4_SHOP.Hubs.NotificationHub>("/notifyHub");
}
