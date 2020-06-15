namespace DevelopersChallenge2.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bank",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.String(),
                        DataReg = c.DateTime(nullable: false),
                        EstReg = c.String(maxLength: 1),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transaction",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        IdBank = c.Long(nullable: false),
                        TransactionType = c.Int(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 10, scale: 2),
                        Description = c.String(maxLength: 96),
                        Hash = c.String(maxLength: 64),
                        DataReg = c.DateTime(nullable: false),
                        EstReg = c.String(maxLength: 1),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bank", t => t.IdBank)
                .Index(t => t.IdBank);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transaction", "IdBank", "dbo.Bank");
            DropIndex("dbo.Transaction", new[] { "IdBank" });
            DropTable("dbo.Transaction");
            DropTable("dbo.Bank");
        }
    }
}
