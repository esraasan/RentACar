using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using RentACar.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); 


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 
builder.Services.AddScoped<ICarsRepository, CarsRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();




builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
    options.LoginPath = "/Users/Login";
    options.AccessDeniedPath = "/Users/Login";
});
builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("type", "Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireClaim("type", "Admin", "Standart"));
});

builder.Services.AddSession();

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum zaman aþýmýný 30 dakika olarak ayarlar
    options.Cookie.HttpOnly = true; //Çerezlerin sadece HTTP üzerinden eriþilebilir olmasýný saðlar
    options.Cookie.IsEssential = true; // Make the session cookie essential (Çerezin önemli olduðunu belirtir)
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Hata yönetimi için özel bir sayfa kullanýr
    app.UseHsts(); // HTTP Strict Transport Security kullanýr
}

app.UseHttpsRedirection(); // HTTP isteklerini HTTPS'e yönlendirir
app.UseStaticFiles(); // Statik dosya servislerini etkinleþtirir
app.UseRouting(); // Yönlendirme özelliklerini etkinleþtirir

// 
app.UseSession(); // Oturum yönetimini etkinleþtirir
app.UseAuthentication();
app.UseAuthorization(); // Yetkilendirme mekanizmasýný etkinleþtirir

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Rota þemasý

app.Run(); 
