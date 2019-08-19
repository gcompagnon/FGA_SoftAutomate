namespace FGABusinessComponent.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IndexValuation_BaseValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("ref_holding.VALUATION", "IndexBaseDate", c => c.DateTime());
            AddColumn("ref_holding.VALUATION", "IndexBaseValue", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("ref_holding.VALUATION", "IndexBaseValue");
            DropColumn("ref_holding.VALUATION", "IndexBaseDate");
        }
    }
}
