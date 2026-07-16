using LaptopCart.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LaptopCart
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container. 
            builder.Services.AddControllersWithViews();

            //Register Services
            // Register the ApplicationDbContext with the connection string from appsettings.json
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("LaptopDbConnection")));
            // Register Identity services and configure it to use the ApplicationDbContext for storing user information
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            async Task SeedRoles(IServiceProvider serviceProvider)
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roleNames = { "Admin","Manager", "User" };
                //IdentityResult roleResult;
                foreach (var roleName in roleNames)
                {
                    //var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (! await roleManager.RoleExistsAsync(roleName))
                    {
                        // Create the roles and seed them to the database
                         await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                SeedRoles(services).Wait();
            }
            app.UseSession();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Product}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
