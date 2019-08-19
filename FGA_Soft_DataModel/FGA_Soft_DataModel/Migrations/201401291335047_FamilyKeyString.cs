namespace FGABusinessComponent.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FamilyKeyString : DbMigration
    {
        public override void Up()
        {
            AddColumn("ref_security.EQUITY", "FamilyKey", c => c.String());
            AddColumn("ref_holding.INDEX", "FamilyKey", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("ref_holding.INDEX", "FamilyKey");
            DropColumn("ref_security.EQUITY", "FamilyKey");
        }
    }
}
