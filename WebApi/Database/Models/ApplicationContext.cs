using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Core.Authorization;

namespace WebApi.Database.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        //public DbSet<SubCategory> SubCategories { get; set; }
        //public DbSet<Currency> Currencies { get; set; }
        //public DbSet<Check> Checks { get; set; }
        //public DbSet<Category> Categories { get; set; }
        //public DbSet<AccountType> AccountTypes { get; set; }
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string moderatorRoleName = "moderator";
            string userRoleName = "user";

            string adminEmail = "kuvshinchikov7127@mail.ru";
            string adminPassword = AuthUtils.GetHash("pasha7127");

            string userEmail = "hellsangel.alt@mail.ru";
            string userPassword = AuthUtils.GetHash("id48151627");

            // добавляем роли
            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role moderatorRole = new Role { Id = 2, Name = moderatorRoleName };
            Role userRole = new Role { Id = 3, Name = userRoleName };

            User adminUser = new User { Id = 1, Login = "BlurTrash", FirstName = "Павел", SecondName = "Кувшинчиков", Email = adminEmail, Password = adminPassword, RoleId = adminRole.Id };
            User testUser = new User { Id = 2, Login = "Vlado", FirstName = "Ваня", SecondName = "Пупкин", Email = userEmail, Password = userPassword, RoleId = userRole.Id };


            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, moderatorRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser, testUser });
            base.OnModelCreating(modelBuilder);
        }
    }
}
