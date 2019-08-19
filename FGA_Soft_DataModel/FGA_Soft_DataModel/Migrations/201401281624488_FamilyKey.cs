namespace FGABusinessComponent.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FamilyKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("ref_holding.VALUATION", "IndexDivisor", c => c.Double());
            AddColumn("ref_holding.VALUATION", "IndexNumberOfSecurities", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("ref_holding.VALUATION", "IndexNumberOfSecurities");
            DropColumn("ref_holding.VALUATION", "IndexDivisor");
        }
    }
}
