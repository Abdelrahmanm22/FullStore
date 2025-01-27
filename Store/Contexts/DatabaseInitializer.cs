using Microsoft.AspNetCore.Identity;
using Store.Models;

namespace Store.Contexts
{
    public class DatabaseInitializer
    {
        public static async Task SeedDataAsync(UserManager<User>? _userManager,RoleManager<IdentityRole>? _roleManager)
        {
            if (_userManager == null || _roleManager == null)
            {
                Console.WriteLine("UserManager or RoleManager is null => exit");
                return;
            }

            ///Check if we have the admin role or not
            var exists = await _roleManager.RoleExistsAsync("admin");
            if (!exists)
            {
                Console.WriteLine("Admin role is not defiend and will be created");
                await _roleManager.CreateAsync(new IdentityRole("admin"));
            }

            ///Check if we have the seller role or not
            exists = await _roleManager.RoleExistsAsync("seller");
            if (!exists)
            {
                Console.WriteLine("Seller role is not defiend and will be created");
                await _roleManager.CreateAsync(new IdentityRole("seller"));
            }

            ///Check if we have the seller role or not
            exists = await _roleManager.RoleExistsAsync("client");
            if (!exists)
            {
                Console.WriteLine("Client role is not defiend and will be created");
                await _roleManager.CreateAsync(new IdentityRole("client"));
            }


            ///Now I want create admin user
            //Check if we have at least one admin user or not
            var adminUser = await _userManager.GetUsersInRoleAsync("admin");
            if (adminUser.Any()) {
                Console.WriteLine("Admin user already exists => exit");
                return;
            }
            //Create the admin user
            var user = new User()
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                CreatedAt = DateTime.Now,
            };
            string initialPassword = "admin123";

            var result = await _userManager.CreateAsync(user,initialPassword);
            if (result.Succeeded) {
                //set the user role
                await _userManager.AddToRoleAsync(user, "admin");
                Console.WriteLine("Admin user created successfully! Please update the initial password!");
                Console.WriteLine("Email: "+user.Email);
                Console.WriteLine("Initial Password: "+initialPassword);
            }
        }
    }
}
