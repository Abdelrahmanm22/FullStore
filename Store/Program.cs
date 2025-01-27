using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sib_api_v3_sdk.Client;
using Store.Contexts;
using Store.MappingProfiles;
using Store.Models;

namespace Store
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddAutoMapper(m => m.AddProfile(new ProductProfile())); //Transient

            builder.Services.AddIdentity<User, IdentityRole>(Options =>
            {
                Options.Password.RequiredLength = 6;
                Options.Password.RequireNonAlphanumeric = false;
                Options.Password.RequireLowercase = false;
                Options.Password.RequireUppercase = false;

            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");



            //Create the roles and the first admin user if not available yet
            using (var scope = app.Services.CreateScope()) {
                var userManager = scope.ServiceProvider.GetService(typeof(UserManager<User>))
                    as UserManager<User>;
                var roleManager = scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>))
                    as RoleManager<IdentityRole>;

                await DatabaseInitializer.SeedDataAsync(userManager, roleManager);
            }

            app.Run();
        }
    }
}
