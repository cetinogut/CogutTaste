using CogutTaste.Models;
using CogutTaste.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CogutTaste.DataAccess.Data.Initializer
{
    public class DbInitializer : IDbInitializer
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager; // rooleri oluşturacağız
            _userManager = userManager; // bir admin user i oluşturup buna admin rolu vereceğiz
        }


        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
            }

            if (_db.Roles.Any(r => r.Name == StaticValues.ManagerRole)) return; // role zaten olduğundan geri dön.. yoksa -ilk defada yapar- rolleri oluştur...

            _roleManager.CreateAsync(new IdentityRole(StaticValues.ManagerRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticValues.FrontDeskRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticValues.KitchenRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticValues.CustomerRole)).GetAwaiter().GetResult();


            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "cogut@gmail.com",
                Email = "cogut@gmail.com",
                EmailConfirmed = true,
                FirstName = "Cetin",
                LastName = "OGUT"
            }, "251609Hc/*").GetAwaiter().GetResult(); // yapıldığından emin olmak için getresul kullandık..

            ApplicationUser user = _db.ApplicationUser.Where
                (u => u.Email == "cogut@gmail.com").FirstOrDefault();
            
            _userManager.AddToRoleAsync(user, StaticValues.ManagerRole).GetAwaiter().GetResult();
        }
    }
}
