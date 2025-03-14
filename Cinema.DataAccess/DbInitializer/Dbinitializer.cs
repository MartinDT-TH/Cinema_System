
using Cinema.DataAccess.Data;
using Cinema.DbInitializer;
using Cinema.Models;
using Cinema.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cinema.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }


        public void Initialize()
        {


            //migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }


            if(!_roleManager.RoleExistsAsync(SD.Role_Guest).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Staff)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Guest)).GetAwaiter().GetResult();


                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FullName = "Admin",
                    PhoneNumber = "1112223333",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                }, "Admin@123").GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "staff@gmail.com",
                    Email = "staff@gmail.com",
                    FullName = "Staff",
                    PhoneNumber = "1112223333",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                }, "Staff@123").GetAwaiter().GetResult();

                var users = new List<ApplicationUser>
                {
                    new ApplicationUser { UserName = "user1@gmail.com", Email = "user1@gmail.com", FullName = "User One", PhoneNumber = "1112223334", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user2@gmail.com", Email = "user2@gmail.com", FullName = "User Two", PhoneNumber = "1112223335", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user3@gmail.com", Email = "user3@gmail.com", FullName = "User Three", PhoneNumber = "1112223336", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user4@gmail.com", Email = "user4@gmail.com", FullName = "User Four", PhoneNumber = "1112223337", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user5@gmail.com", Email = "user5@gmail.com", FullName = "User Five", PhoneNumber = "1112223338", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user6@gmail.com", Email = "user6@gmail.com", FullName = "User Six", PhoneNumber = "1112223339", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user7@gmail.com", Email = "user7@gmail.com", FullName = "User Seven", PhoneNumber = "1112223340", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user8@gmail.com", Email = "user8@gmail.com", FullName = "User Eight", PhoneNumber = "1112223341", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user9@gmail.com", Email = "user9@gmail.com", FullName = "User Nine", PhoneNumber = "1112223342", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user10@gmail.com", Email = "user10@gmail.com", FullName = "User Ten", PhoneNumber = "1112223343", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user11@gmail.com", Email = "user11@gmail.com", FullName = "User Eleven", PhoneNumber = "1112223344", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user12@gmail.com", Email = "user12@gmail.com", FullName = "User Twelve", PhoneNumber = "1112223345", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user13@gmail.com", Email = "user13@gmail.com", FullName = "User Thirteen", PhoneNumber = "1112223346", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user14@gmail.com", Email = "user14@gmail.com", FullName = "User Fourteen", PhoneNumber = "1112223347", EmailConfirmed = true, PhoneNumberConfirmed = true },
                    new ApplicationUser { UserName = "user15@gmail.com", Email = "user15@gmail.com", FullName = "User Fifteen", PhoneNumber = "1112223348", EmailConfirmed = true, PhoneNumberConfirmed = true }
                };

                foreach (var aUser in users)
                {
                    _userManager.CreateAsync(aUser, "User@123").GetAwaiter().GetResult();
                }

                //"123*" is too weak (many ASP.NET Identity systems require at least one 
                //    uppercase letter, one lowercase letter, a number, and a special character). -> can not create user

                // Refresh the user context
                _db.SaveChanges();  // Ensure database changes are committed
                var user = _userManager.FindByEmailAsync("admin@gmail.com").GetAwaiter().GetResult();
                var user2 = _userManager.FindByEmailAsync("staff@gmail.com").GetAwaiter().GetResult();

                if (user != null)
                {
                    _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
                }
                if (user2 != null)
                {
                    _userManager.AddToRoleAsync(user2, SD.Role_Staff).GetAwaiter().GetResult();
                }
            }



        }
    }
}