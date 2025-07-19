using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TL4_SHOP.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<Tl4ShopContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TL4_SHOP"));
});

// 2. Add authentication providers (Google, Facebook)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(googleOptions =>
{
    // TODO: Replace with your real Google API keys
    googleOptions.ClientId = "115282379706-5u2q3s4nuakf0f6ljs1fjuu9nu1nk04n.apps.googleusercontent.com";
    googleOptions.ClientSecret = "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";
})
.AddFacebook(facebookOptions =>
{
    // TODO: Replace with your real Facebook App credentials
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
