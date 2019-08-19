namespace FGABusinessComponent.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurrencyExchange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ref_security.FX_RATE",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        UnitCurrency_Currency = c.String(maxLength: 4, fixedLength: true),
                        QuotedCurrency_Currency = c.String(maxLength: 4, fixedLength: true),
                        FX = c.Double(),
                    })
                .PrimaryKey(t => new { t.Id, t.Date });
            
        }
        
        public override void Down()
        {
            DropTable("ref_security.FX_RATE");
        }
    }
}
