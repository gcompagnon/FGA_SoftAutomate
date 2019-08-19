namespace FGABusinessComponent.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValuationSource : DbMigration
    {
        public override void Up()
        {
            AddColumn("ref_holding.VALUATION", "ValuationSource", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("ref_holding.VALUATION", "ValuationSource");
        }
    }
}
