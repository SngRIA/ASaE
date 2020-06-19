using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ASaE.Models
{
    public class ASaEDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override async void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var role1 = new IdentityRole { Name = "admin" };
            var role2 = new IdentityRole { Name = "user" };
            var roleEas = new IdentityRole { Name = "eas" };
            var roleSas = new IdentityRole { Name = "sas" };

            roleManager.Create(role1);
            roleManager.Create(role2);
            roleManager.Create(roleEas);
            roleManager.Create(roleSas);

            var admin = new ApplicationUser { Email = "admin@cmit.ru", UserName = "admin@cmit.ru", FirstName = "Администратор", LastName = "Администратор", 
                MiddleName = "Администратор", UserBasket = new UserBasket() };

            var employeeSAS = new ApplicationUser { Email = "employeeSas@cmit.ru", UserName = "employeeSas@cmit.ru", FirstName = "Караев", LastName = "Кузьма", 
                MiddleName = "Игоревич", UserBasket = new UserBasket() };

            var employeeEAS = new ApplicationUser { Email = "employeeEas@cmit.ru", UserName = "employeeEas@cmit.ru", FirstName = "Шуев", LastName = "Александр", 
                MiddleName = "Николаевич", UserBasket = new UserBasket() };

            var result1 = userManager.Create(admin, "password1");
            if (result1.Succeeded)
            {
                userManager.AddToRole(admin.Id, role1.Name);
                userManager.AddToRole(admin.Id, role2.Name);
                userManager.AddToRole(admin.Id, roleEas.Name);
                userManager.AddToRole(admin.Id, roleSas.Name);
            }

            var result2 = userManager.Create(employeeSAS, "password1");
            if (result2.Succeeded)
            {
                userManager.AddToRole(employeeSAS.Id, roleSas.Name);
            }

            var result3 = userManager.Create(employeeEAS, "password1");
            if (result3.Succeeded)
            {
                userManager.AddToRole(employeeEAS.Id, roleEas.Name);
                context.Employees.Add(new Employee
                {
                    Address = "Проспект тукая 2",
                    User = employeeEAS
                });
            }

            base.Seed(context);
            context.SaveChanges();
        }
    }
}