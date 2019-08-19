namespace FGABusinessComponent.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValuationSource_FX : DbMigration
    {
        public override void Up()
        {
            AddColumn("ref_security.FX_RATE", "ValuationSource", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("ref_security.FX_RATE", "ValuationSource");
        }
    }
}
