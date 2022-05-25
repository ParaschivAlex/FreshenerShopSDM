namespace FreshenerShopSDM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateInitial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        CartId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CartId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ItemCarts",
                c => new
                    {
                        ItemCartId = c.Int(nullable: false, identity: true),
                        FreshenerId = c.Int(nullable: false),
                        CartId = c.Int(nullable: false),
                        OrderId = c.Int(),
                        ItemCartQuantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ItemCartId)
                .ForeignKey("dbo.Carts", t => t.CartId, cascadeDelete: true)
                .ForeignKey("dbo.Fresheners", t => t.FreshenerId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .Index(t => t.FreshenerId)
                .Index(t => t.CartId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Fresheners",
                c => new
                    {
                        FreshenerId = c.Int(nullable: false, identity: true),
                        FreshenerName = c.String(nullable: false, maxLength: 128),
                        FreshenerSlug = c.String(maxLength: 256),
                        FreshenerDescription = c.String(nullable: false),
                        FreshenerPrice = c.Single(nullable: false),
                        FreshenerRating = c.Single(nullable: false),
                        FreshenerImage = c.String(nullable: false),
                        FreshenerAvailability = c.Boolean(nullable: false),
                        FreshenerCode = c.String(),
                        FreshenerStock = c.Int(nullable: false),
                        FreshenerModifyDate = c.DateTime(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.FreshenerId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.CategoryId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(maxLength: 128),
                        CategorySlug = c.String(maxLength: 256),
                        CategoryImage = c.String(nullable: false),
                        CategoryDescription = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.OrderCompletes",
                c => new
                    {
                        OrderCompleteId = c.Int(nullable: false, identity: true),
                        FreshenerId = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                        FreshenerQuantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderCompleteId)
                .ForeignKey("dbo.Fresheners", t => t.FreshenerId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.FreshenerId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        OrderUsername = c.String(),
                        OrderFirstName = c.String(nullable: false, maxLength: 64),
                        OrderLastName = c.String(nullable: false, maxLength: 64),
                        OrderCity = c.String(nullable: false, maxLength: 128),
                        OrderState = c.String(nullable: false, maxLength: 128),
                        OrderStreet = c.String(nullable: false, maxLength: 256),
                        OrderPostalCode = c.String(),
                        OrderPhone = c.String(nullable: false),
                        OrderEmail = c.String(nullable: false),
                        OrderTotal = c.Single(nullable: false),
                        OrderModifyDate = c.DateTime(nullable: false),
                        OrderSent = c.Boolean(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        ReviewId = c.Int(nullable: false, identity: true),
                        ReviewComment = c.String(maxLength: 2048),
                        ReviewGrade = c.Int(nullable: false),
                        ReviewModifyDate = c.DateTime(nullable: false),
                        FreshenerId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.Fresheners", t => t.FreshenerId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.FreshenerId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        ContactId = c.Int(nullable: false, identity: true),
                        ContactName = c.String(nullable: false, maxLength: 64),
                        ContactEmail = c.String(nullable: false, maxLength: 64),
                        ContactPhoneNumber = c.String(maxLength: 16),
                        ContactSubject = c.String(nullable: false, maxLength: 32),
                        ContactMessage = c.String(nullable: false, maxLength: 2048),
                        ContactModifyDate = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ContactId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Contacts", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Carts", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Fresheners", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "FreshenerId", "dbo.Fresheners");
            DropForeignKey("dbo.Orders", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderCompletes", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.ItemCarts", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderCompletes", "FreshenerId", "dbo.Fresheners");
            DropForeignKey("dbo.ItemCarts", "FreshenerId", "dbo.Fresheners");
            DropForeignKey("dbo.Fresheners", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.ItemCarts", "CartId", "dbo.Carts");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Contacts", new[] { "UserId" });
            DropIndex("dbo.Reviews", new[] { "UserId" });
            DropIndex("dbo.Reviews", new[] { "FreshenerId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.OrderCompletes", new[] { "OrderId" });
            DropIndex("dbo.OrderCompletes", new[] { "FreshenerId" });
            DropIndex("dbo.Fresheners", new[] { "UserId" });
            DropIndex("dbo.Fresheners", new[] { "CategoryId" });
            DropIndex("dbo.ItemCarts", new[] { "OrderId" });
            DropIndex("dbo.ItemCarts", new[] { "CartId" });
            DropIndex("dbo.ItemCarts", new[] { "FreshenerId" });
            DropIndex("dbo.Carts", new[] { "UserId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Contacts");
            DropTable("dbo.Reviews");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderCompletes");
            DropTable("dbo.Categories");
            DropTable("dbo.Fresheners");
            DropTable("dbo.ItemCarts");
            DropTable("dbo.Carts");
        }
    }
}
