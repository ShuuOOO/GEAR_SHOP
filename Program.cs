using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<_4tlShopContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("4TL_SHOP"));
});
//2.Add authentication providers(Google, Facebook) và phần của thầy Hiển

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = "115282379706-...";
    googleOptions.ClientSecret = "GOCSPX-...";
})
.AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = "FACEBOOK_APP_ID";
    facebookOptions.AppSecret = "FACEBOOK_APP_SECRET";
});

// 3. Build the app
var app = builder.Build();

// 4. Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

app.UseSession();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
