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
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum zaman a��m�n� 30 dakika olarak ayarlar
    options.Cookie.HttpOnly = true; //�erezlerin sadece HTTP �zerinden eri�ilebilir olmas�n� sa�lar
    options.Cookie.IsEssential = true; // Make the session cookie essential (�erezin �nemli oldu�unu belirtir)
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Hata y�netimi i�in �zel bir sayfa kullan�r
    app.UseHsts(); // HTTP Strict Transport Security kullan�r
}

app.UseHttpsRedirection(); // HTTP isteklerini HTTPS'e y�nlendirir
app.UseStaticFiles(); // Statik dosya servislerini etkinle�tirir
app.UseRouting(); // Y�nlendirme �zelliklerini etkinle�tirir

// 
app.UseSession(); // Oturum y�netimini etkinle�tirir
app.UseAuthentication();
app.UseAuthorization(); // Yetkilendirme mekanizmas�n� etkinle�tirir

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Rota �emas�

app.Run(); 
