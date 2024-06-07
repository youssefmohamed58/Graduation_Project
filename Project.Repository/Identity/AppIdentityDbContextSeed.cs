using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Project.Core.Entities;

namespace Project.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> usermanager)
        {
            if (!usermanager.Users.Any())
            {
                var user = new AppUser()
                {
                    FullName = "Youssef Mohamed",
                    Email = "youssefmohamed@gmail.com",
                    UserName = "youssefmohamed",
                    PhoneNumber = "01200656276",
                };
                await usermanager.CreateAsync(user, "Pa$$w0rd");

            }
        }
    }
}
