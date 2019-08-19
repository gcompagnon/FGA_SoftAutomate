namespace FGABusinessComponent.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FamilyKey2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "ref_security.PRICE", name: "PriceSource", newName: "Price_Source");
            RenameColumn(table: "ref_security.FX_RATE", name: "UnitCurrency_Currency", newName: "UnitCurrency");
            RenameColumn(table: "ref_security.FX_RATE", name: "QuotedCurrency_Currency", newName: "QuotedCurrency");
            AddColumn("ref_security.DEBT", "FamilyKey", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("ref_security.DEBT", "FamilyKey");
            RenameColumn(table: "ref_security.FX_RATE", name: "QuotedCurrency", newName: "QuotedCurrency_Currency");
            RenameColumn(table: "ref_security.FX_RATE", name: "UnitCurrency", newName: "UnitCurrency_Currency");
            RenameColumn(table: "ref_security.PRICE", name: "Price_Source", newName: "PriceSource");
        }
    }
}
