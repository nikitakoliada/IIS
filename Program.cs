using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IIS.Data;
using IIS.Models;
using IIS.Repositories;
using IIS.Services.Abstractions;
using IIS.Services.Implementations;


async Task CreateRoles(RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "Admin", "StudioAdmin", "Teacher", "Student" };
    IdentityResult roleResult;

    foreach (var roleName in roleNames)
    {
        // Check if the role exists
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            // Create the roles and seed them to the database
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

bool IsLocalEnvironment()
{
    return Environment.GetEnvironmentVariable("IsLocalEnvironment") == "true";
}


var builder = WebApplication.CreateBuilder(args);
var connectionStringName = IsLocalEnvironment() ? "DefaultConnection" : "AzureConnection";
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString(connectionStringName) ??
                       throw new InvalidOperationException($"Connection string {connectionStringName} not found.");

if (builder.Environment.IsDevelopment() && IsLocalEnvironment())
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()  // Enable roles
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// Add repositories
builder.Services.AddScoped<EquipmentTypeRepository>();
builder.Services.AddScoped<RentalDayIntervalRepository>();
builder.Services.AddScoped<StudioRepository>();
builder.Services.AddScoped<EquipmentRepository>();
builder.Services.AddScoped<BorrowRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IBorrowService, BorrowService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await CreateRoles(scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>());

    await SeedData.Initialize(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();