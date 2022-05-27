using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FreshenerShopSDM.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

		public string Name { get; set; }

		public IEnumerable<SelectListItem> AllRoles { get; set; }

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /*public ApplicationDbContext()
             : base("DefaultConnection", throwIfV1Schema: false)
         {
             Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext,
                 FreshenerShopSDM.Migrations.Configuration>("DefaultConnection"));
        }*/

        //pentru a reveni la baza de date locala, decomentati codul de mai sus si comentati-l pe cel de mai jos.
        //eventual este necesara stergerea migratiei anterioare, apoi comanda enable-migrations -Force, Add-Migration Initial (-IgnoreChanges) si Update-Database
        public ApplicationDbContext()
            : base("AzureDb", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext,
                FreshenerShopSDM.Migrations.Configuration>("AzureDb"));
        //Pentru a deschide db-ul dintr-un loc fizic nou, azure portal -> sql database -> firewall rules -> add a firewall rule -> save
        }

        /*La schimbarea db-ului din local in azure, trebuie sa nu fie populat fisierul de migratie.
         Daca este populat cu toata migratia, visual studio ruleaza acel fisier la fiecare schimbare.
         Astfel apare o eroare la primul rand (ex. are create table Carts dar deja exista).
         Singura solutie este sa comentez fisierul de migratie cand folosesc db-ul de azure. Sau daca am nevoie de migratie
         pe db-ul de azure, trebuie sters fisierul de migratie si creat altul pe acel connectionString.*/

        public DbSet<Freshener> Fresheners { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<ItemCart> ItemCarts { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<OrderComplete> OrderCompletes { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}