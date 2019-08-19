namespace FGABusinessComponent.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YieldValorisation : DbMigration
    {
        public override void Up()
        {
            AddColumn("ref_holding.VALUATION", "Yield_ChangePrice_1D_Value", c => c.Double());
            AddColumn("ref_holding.VALUATION", "Yield_ChangePrice_MTD_Value", c => c.Double());
            AddColumn("ref_holding.VALUATION", "Yield_ChangePrice_YTD_Value", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("ref_holding.VALUATION", "Yield_ChangePrice_YTD_Value");
            DropColumn("ref_holding.VALUATION", "Yield_ChangePrice_MTD_Value");
            DropColumn("ref_holding.VALUATION", "Yield_ChangePrice_1D_Value");
        }
    }
}
