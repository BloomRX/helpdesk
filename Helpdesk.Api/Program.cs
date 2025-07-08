using Helpdesk.Api.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt;

var builder = WebApplication.CreateBuilder(args);

// Configurações
var configuration = builder.Configuration;

// Serviços
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Banco de Dados (SQLite para desenvolvimento)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
    
    // Para usar SQL Server em produção:
    // options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection"));
});
builder.Services.Configure<BcryptSettings>(builder.Configuration.GetSection("Bcrypt"));

var app = builder.Build();

// Migrações automáticas (apenas em desenvolvimento)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();