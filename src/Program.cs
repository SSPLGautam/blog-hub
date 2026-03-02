using System.Linq.Expressions;
using BlogApp.Data;
using BlogApp.Repository;
using BlogApp.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();                                      
builder.Services.ConfigureApplicationCookie(options =>

{ 
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;

});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin"));
});

//Add  Services for Repository and Service
// Generic Repo
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Custom Repos 
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();

// Services
builder.Services.AddScoped<IPostService, PostService>();          
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
var app = builder.Build();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { "Admin", "Author", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
catch (Exception ex)
{
    // log error if needed
}

try     
{
    using (var scope = app.Services.CreateScope())
    {
        var _userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        var _roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        string adminEmail = "admin@gmail.com";
        string adminPassword = "Admin@1234";

        var existingAdminRole = await _roleManager.FindByNameAsync("Admin");
        if (existingAdminRole == null)
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        var existingAdminUser = await _userManager.FindByEmailAsync(adminEmail);
        if (existingAdminUser == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                throw new Exception("Could not create Admin user.");
            }
        }
    }
}
catch (Exception ex)
{

}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
