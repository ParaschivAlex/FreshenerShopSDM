namespace FreshenerShopSDM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(maxLength: 128),
                        CategorySlug = c.String(maxLength: 256),
                        CategoryImage = c.String(nullable: false),
                        CategoryDescription = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
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
                "dbo.Reviews",
                c => new
                    {
                        ReviewId = c.Int(nullable: false, identity: true),
                        ReviewComment = c.String(maxLength: 512),
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
            
            AddColumn("dbo.AspNetUsers", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Fresheners", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reviews", "FreshenerId", "dbo.Fresheners");
            DropForeignKey("dbo.Fresheners", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Reviews", new[] { "UserId" });
            DropIndex("dbo.Reviews", new[] { "FreshenerId" });
            DropIndex("dbo.Fresheners", new[] { "UserId" });
            DropIndex("dbo.Fresheners", new[] { "CategoryId" });
            DropColumn("dbo.AspNetUsers", "Name");
            DropTable("dbo.Reviews");
            DropTable("dbo.Fresheners");
            DropTable("dbo.Categories");
        }
    }
}
