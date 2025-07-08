using Microsoft.AspNetCore.Authentication.Cookies;
using Helpdesk.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Conexão com SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=helpdesk.db"));

// Autenticação por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuarios/Login";
        options.LogoutPath = "/Usuarios/Logout";
    });

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Migrar banco ao iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession();

// IMPORTANTE
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuarios}/{action=Login}/{id?}");

app.Run();
