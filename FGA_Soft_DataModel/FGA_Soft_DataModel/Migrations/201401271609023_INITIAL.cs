namespace FGABusinessComponent.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class INITIAL : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ref_security.ASSET_CLASSIFICATION",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Source = c.String(nullable: false, maxLength: 128),
                        Classification1 = c.String(),
                        Classification2 = c.String(),
                        Classification3 = c.String(),
                        Classification4 = c.String(),
                        Classification5 = c.String(),
                        Classification6 = c.String(),
                        Classification7 = c.String(),
                        AssetId = c.Long(nullable: false),
                        ISINId = c.String(nullable: false, maxLength: 12, fixedLength: true),
                    })
                .PrimaryKey(t => new { t.Id, t.Source })
                .ForeignKey("ref_security.ASSET", t => new { t.AssetId, t.ISINId }, cascadeDelete: true)
                .Index(t => new { t.AssetId, t.ISINId });
            
            CreateTable(
                "ref_issuer.ROLE",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AssetId = c.Long(nullable: false),
                        ISINId = c.String(nullable: false, maxLength: 12, fixedLength: true),
                        Country = c.String(maxLength: 2, fixedLength: true),
                        IssuerName = c.String(maxLength: 60),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("ref_security.ASSET", t => new { t.AssetId, t.ISINId }, cascadeDelete: true)
                .Index(t => new { t.AssetId, t.ISINId });
            
            CreateTable(
                "ref_holding.COMPONENT",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ISIN = c.String(nullable: false, maxLength: 12, fixedLength: true),
                        Date = c.DateTime(nullable: false),
                        ParentId = c.Long(),
                        ParentISIN = c.String(maxLength: 12, fixedLength: true),
                        ParentDate = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.Id, t.ISIN, t.Date })
                .ForeignKey("ref_holding.COMPONENT", t => new { t.ParentId, t.ParentISIN, t.ParentDate })
                .Index(t => new { t.ParentId, t.ParentISIN, t.ParentDate });
            
            CreateTable(
                "ref_holding.VALUATION",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        FaceAmount = c.Double(),
                        FaceAmount_Cur = c.String(maxLength: 4, fixedLength: true),
                        MarketValue = c.Double(),
                        MarketValue_Cur = c.String(maxLength: 4, fixedLength: true),
                        BookValue = c.Double(),
                        BookValue_Cur = c.String(maxLength: 4, fixedLength: true),
                        Debt_Duration = c.Double(),
                        Debt_Sensitivity = c.Double(),
                        Debt_TTM = c.Double(),
                        Debt_Convexity = c.Double(),
                        DebtDataCalculation_MacaulayDurationSemiAnnual = c.Double(),
                        DebtDataCalculation_ModifiedDurationSemiAnnual = c.Double(),
                        DebtDataCalculation_ConvexitySemiAnnual = c.Double(),
                        DebtDataCalculation_WeightedAverageLife = c.Double(),
                        Debt_YTM_Rate = c.Double(),
                        DebtYield_YieldToWorstRate_Value = c.Double(),
                        DebtYield_YieldToNextCallRate_Value = c.Double(),
                        DebtSpread_GovSpread = c.Double(),
                        DebtSpread_InterpolatedSwapSpread = c.Double(),
                        DebtSpread_CDSBondBasisSpread = c.Double(),
                        DebtSpread_ZeroVolatilitySpread = c.Double(),
                        DebtSpread_AssetSwapSpread = c.Double(),
                        DebtSpread_OptionAdjustedSpread = c.Double(),
                        ContainerId = c.Long(),
                        ISINId = c.String(maxLength: 12, fixedLength: true),
                        ContainerDate = c.DateTime(nullable: false),
                        IndexValue = c.Double(),
                        NumberOfParts = c.Single(),
                        ValuationType = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Id, t.Date })
                .ForeignKey("ref_holding.COMPONENT", t => new { t.ContainerId, t.ISINId, t.ContainerDate })
                .Index(t => new { t.ContainerId, t.ISINId, t.ContainerDate });
            
            CreateTable(
                "ref_common.IDENTIFICATION",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 350),
                        ISIN = c.String(maxLength: 12, fixedLength: true),
                        Country = c.String(maxLength: 2, fixedLength: true),
                        OtherIdentification = c.String(maxLength: 35),
                        ProprietaryIdentificationSource = c.String(maxLength: 35),
                        Bloomberg = c.String(maxLength: 20, fixedLength: true),
                        Reuters = c.String(maxLength: 20, fixedLength: true),
                        SEDOL = c.String(maxLength: 20, fixedLength: true),
                        CUSIP = c.String(maxLength: 20, fixedLength: true),
                        Ticker = c.String(maxLength: 20, fixedLength: true),
                        TradingIdentification = c.String(maxLength: 70),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "ref_security.ASSET_PORTFOLIO",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvestmentAmount = c.Double(),
                        InvestmentAmount_Cur = c.String(maxLength: 4, fixedLength: true),
                        InvestmentRate = c.Double(),
                        EffectiveDate = c.DateTime(),
                        Asset_Id = c.Long(),
                        Asset_ISINId = c.String(maxLength: 12, fixedLength: true),
                        Portfolio_Id = c.Long(),
                        Portfolio_ISINId = c.String(maxLength: 12, fixedLength: true),
                        Portfolio_Date = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("ref_security.ASSET", t => new { t.Asset_Id, t.Asset_ISINId })
                .ForeignKey("ref_holding.PORTFOLIO", t => new { t.Portfolio_Id, t.Portfolio_ISINId, t.Portfolio_Date })
                .Index(t => new { t.Asset_Id, t.Asset_ISINId })
                .Index(t => new { t.Portfolio_Id, t.Portfolio_ISINId, t.Portfolio_Date });
            
            CreateTable(
                "ref_security.ASSET",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ISIN = c.String(nullable: false, maxLength: 12, fixedLength: true),
                        FinancialInstrumentName = c.String(maxLength: 350),
                        MaturityDate = c.DateTime(),
                        FinancialAssetCategory_FinancialAssetTypeCategory = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        IdentificationId = c.Long(nullable: false),
                        RatingId = c.Long(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Id, t.ISIN })
                .ForeignKey("ref_common.IDENTIFICATION", t => t.IdentificationId, cascadeDelete: true)
                .ForeignKey("ref_rating.RATING", t => t.RatingId)
                .Index(t => t.IdentificationId)
                .Index(t => t.RatingId);
            
            CreateTable(
                "ref_security.SECURITIES_ISSUANCE",
                c => new
                    {
                        SecurityId = c.Long(),
                        Id = c.Long(nullable: false, identity: true),
                        Date = c.DateTime(),
                        Unit = c.Double(),
                        Amount_Value = c.Double(),
                        Amount_Currency_Currency = c.String(maxLength: 4, fixedLength: true),
                        Type_CapitalType = c.String(),
                        ISINId = c.String(maxLength: 12, fixedLength: true),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("ref_security.ASSET", t => new { t.SecurityId, t.ISINId })
                .Index(t => new { t.SecurityId, t.ISINId });
            
            CreateTable(
                "ref_security.PRICE",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Price = c.Double(),
                        Price_Cur = c.String(maxLength: 4, fixedLength: true),
                        Price_Type = c.String(),
                        PriceSource = c.String(),
                        Price_Ask = c.Double(),
                        Price_Bid = c.Double(),
                        Price_Mid = c.Double(),
                        PriceFactType_LastPrice = c.Double(),
                        PriceFactType_BestPrice = c.Double(),
                        PriceFactType_ClosePrice = c.Double(),
                        PriceFactType_OpenPrice = c.Double(),
                        PriceFactType_LowPrice = c.Double(),
                        PriceFactType_HighPrice = c.Double(),
                        Yield_ChangePrice_1D_Value = c.Double(),
                        Yield_ChangePrice_MTD_Value = c.Double(),
                        Yield_ChangePrice_YTD_Value = c.Double(),
                        Debt_CleanP = c.Double(),
                        Debt_DirtyP = c.Double(),
                        Debt_AI = c.Double(),
                        Debt_Duration = c.Double(),
                        Debt_Sensitivity = c.Double(),
                        Debt_TTM = c.Double(),
                        Debt_Convexity = c.Double(),
                        DebtDataCalculation_MacaulayDurationSemiAnnual = c.Double(),
                        DebtDataCalculation_ModifiedDurationSemiAnnual = c.Double(),
                        DebtDataCalculation_ConvexitySemiAnnual = c.Double(),
                        DebtDataCalculation_WeightedAverageLife = c.Double(),
                        Debt_YTM_Rate = c.Double(),
                        DebtYield_YieldToWorstRate_Value = c.Double(),
                        DebtYield_YieldToNextCallRate_Value = c.Double(),
                        DebtSpread_GovSpread = c.Double(),
                        DebtSpread_InterpolatedSwapSpread = c.Double(),
                        DebtSpread_CDSBondBasisSpread = c.Double(),
                        DebtSpread_ZeroVolatilitySpread = c.Double(),
                        DebtSpread_AssetSwapSpread = c.Double(),
                        DebtSpread_OptionAdjustedSpread = c.Double(),
                        SecurityId = c.Long(),
                        ISINId = c.String(maxLength: 12, fixedLength: true),
                    })
                .PrimaryKey(t => new { t.Id, t.Date })
                .ForeignKey("ref_security.ASSET", t => new { t.SecurityId, t.ISINId })
                .Index(t => new { t.SecurityId, t.ISINId });
            
            CreateTable(
                "ref_rating.RATING",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ISINId = c.String(nullable: false, maxLength: 12, fixedLength: true),
                        AssetId = c.Long(nullable: false),
                        Value = c.String(),
                        RatingScheme = c.String(),
                        ValueDate = c.DateTime(),
                        Moody = c.String(),
                        MoodyDate = c.DateTime(),
                        SnP = c.String(),
                        SnPDate = c.DateTime(),
                        Fitch = c.String(),
                        FitchDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("ref_security.ASSET", t => new { t.AssetId, t.ISINId }, cascadeDelete: true)
                .Index(t => new { t.AssetId, t.ISINId });
            
            CreateTable(
                "ref_holding.ASSET_HOLDING",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        ISIN = c.String(nullable: false, maxLength: 12, fixedLength: true),
                        Date = c.DateTime(nullable: false),
                        ParentISIN = c.String(maxLength: 12, fixedLength: true),
                        AssetId = c.Long(nullable: false),
                        AssetISIN = c.String(nullable: false, maxLength: 12, fixedLength: true),
                        FaceAmount = c.Double(),
                        FaceAmount_Cur = c.String(maxLength: 4, fixedLength: true),
                        MarketValue = c.Double(),
                        MarketValue_Cur = c.String(maxLength: 4, fixedLength: true),
                        BookValue = c.Double(),
                        BookValue_Cur = c.String(maxLength: 4, fixedLength: true),
                        Quantity = c.Single(),
                        Weight = c.Double(),
                    })
                .PrimaryKey(t => new { t.Id, t.ISIN, t.Date })
                .ForeignKey("ref_holding.COMPONENT", t => new { t.Id, t.ISIN, t.Date })
                .ForeignKey("ref_security.ASSET", t => new { t.AssetId, t.AssetISIN }, cascadeDelete: true)
                .Index(t => new { t.Id, t.ISIN, t.Date })
                .Index(t => new { t.AssetId, t.AssetISIN });
            
            CreateTable(
                "ref_security.DEBT",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        ISIN = c.String(nullable: false, maxLength: 12, fixedLength: true),
                        NextInterest_CalcFreq = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        NextInterest_Rate = c.Double(),
                        NextInterest_DayCountBasis_InterestComputationMethod = c.String(),
                        FirstPaymentDate = c.DateTime(),
                        PutableDate = c.DateTime(),
                        PutableIndicator = c.Boolean(),
                        SinkableIndicator = c.Boolean(),
                        FixedToVariableIndicator = c.Boolean(),
                        VariableRateIndicator = c.Boolean(),
                        NextCallableDate = c.DateTime(),
                        PerpetualIndicator = c.Boolean(),
                        MinimumIncrement_Rate_Value = c.Double(),
                        MinIncrement = c.Int(),
                        MinimumIncrement_Amount_Value = c.Double(),
                        MinimumIncrement_Amount_Currency_Currency = c.String(maxLength: 4, fixedLength: true),
                        FinancialInfos_Seniority_SeniorityLevel = c.String(maxLength: 13, fixedLength: true, unicode: false),
                        FinancialInfos_HybridCapital = c.Boolean(),
                    })
                .PrimaryKey(t => new { t.Id, t.ISIN })
                .ForeignKey("ref_security.ASSET", t => new { t.Id, t.ISIN })
                .Index(t => new { t.Id, t.ISIN });
            
            CreateTable(
                "ref_security.EQUITY",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        ISIN = c.String(nullable: false, maxLength: 12, fixedLength: true),
                    })
                .PrimaryKey(t => new { t.Id, t.ISIN })
                .ForeignKey("ref_security.ASSET", t => new { t.Id, t.ISIN })
                .Index(t => new { t.Id, t.ISIN });
            
            CreateTable(
                "ref_holding.INDEX",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        ISIN = c.String(nullable: false, maxLength: 12, fixedLength: true),
                        Date = c.DateTime(nullable: false),
                        Name = c.String(maxLength: 350),
                        IdentificationId = c.Long(nullable: false),
                        IndexFixingDate = c.Time(precision: 7),
                        IndexFrequency = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        IndexCurrency = c.String(maxLength: 4, fixedLength: true),
                    })
                .PrimaryKey(t => new { t.Id, t.ISIN, t.Date })
                .ForeignKey("ref_holding.COMPONENT", t => new { t.Id, t.ISIN, t.Date })
                .ForeignKey("ref_common.IDENTIFICATION", t => t.IdentificationId, cascadeDelete: true)
                .Index(t => new { t.Id, t.ISIN, t.Date })
                .Index(t => t.IdentificationId);
            
            CreateTable(
                "ref_security.FUND",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        ISIN = c.String(nullable: false, maxLength: 12, fixedLength: true),
                        DistributionPolicy_DistributionPolicy = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        DividendPolicy_DividendPolicy = c.String(maxLength: 1, fixedLength: true, unicode: false),
                    })
                .PrimaryKey(t => new { t.Id, t.ISIN })
                .ForeignKey("ref_security.ASSET", t => new { t.Id, t.ISIN })
                .Index(t => new { t.Id, t.ISIN });
            
            CreateTable(
                "ref_holding.PORTFOLIO",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        ISIN = c.String(nullable: false, maxLength: 12, fixedLength: true),
                        Date = c.DateTime(nullable: false),
                        Name = c.String(maxLength: 350),
                        IdentificationId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Id, t.ISIN, t.Date })
                .ForeignKey("ref_holding.COMPONENT", t => new { t.Id, t.ISIN, t.Date })
                .ForeignKey("ref_common.IDENTIFICATION", t => t.IdentificationId, cascadeDelete: true)
                .Index(t => new { t.Id, t.ISIN, t.Date })
                .Index(t => t.IdentificationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("ref_holding.PORTFOLIO", "IdentificationId", "ref_common.IDENTIFICATION");
            DropForeignKey("ref_holding.PORTFOLIO", new[] { "Id", "ISIN", "Date" }, "ref_holding.COMPONENT");
            DropForeignKey("ref_security.FUND", new[] { "Id", "ISIN" }, "ref_security.ASSET");
            DropForeignKey("ref_holding.INDEX", "IdentificationId", "ref_common.IDENTIFICATION");
            DropForeignKey("ref_holding.INDEX", new[] { "Id", "ISIN", "Date" }, "ref_holding.COMPONENT");
            DropForeignKey("ref_security.EQUITY", new[] { "Id", "ISIN" }, "ref_security.ASSET");
            DropForeignKey("ref_security.DEBT", new[] { "Id", "ISIN" }, "ref_security.ASSET");
            DropForeignKey("ref_holding.ASSET_HOLDING", new[] { "AssetId", "AssetISIN" }, "ref_security.ASSET");
            DropForeignKey("ref_holding.ASSET_HOLDING", new[] { "Id", "ISIN", "Date" }, "ref_holding.COMPONENT");
            DropForeignKey("ref_security.ASSET_CLASSIFICATION", new[] { "AssetId", "ISINId" }, "ref_security.ASSET");
            DropForeignKey("ref_security.ASSET", "RatingId", "ref_rating.RATING");
            DropForeignKey("ref_rating.RATING", new[] { "AssetId", "ISINId" }, "ref_security.ASSET");
            DropForeignKey("ref_security.PRICE", new[] { "SecurityId", "ISINId" }, "ref_security.ASSET");
            DropForeignKey("ref_security.ASSET", "IdentificationId", "ref_common.IDENTIFICATION");
            DropForeignKey("ref_security.SECURITIES_ISSUANCE", new[] { "SecurityId", "ISINId" }, "ref_security.ASSET");
            DropForeignKey("ref_security.ASSET_PORTFOLIO", new[] { "Portfolio_Id", "Portfolio_ISINId", "Portfolio_Date" }, "ref_holding.PORTFOLIO");
            DropForeignKey("ref_security.ASSET_PORTFOLIO", new[] { "Asset_Id", "Asset_ISINId" }, "ref_security.ASSET");
            DropForeignKey("ref_holding.VALUATION", new[] { "ContainerId", "ISINId", "ContainerDate" }, "ref_holding.COMPONENT");
            DropForeignKey("ref_holding.COMPONENT", new[] { "ParentId", "ParentISIN", "ParentDate" }, "ref_holding.COMPONENT");
            DropForeignKey("ref_issuer.ROLE", new[] { "AssetId", "ISINId" }, "ref_security.ASSET");
            DropIndex("ref_holding.PORTFOLIO", new[] { "IdentificationId" });
            DropIndex("ref_holding.PORTFOLIO", new[] { "Id", "ISIN", "Date" });
            DropIndex("ref_security.FUND", new[] { "Id", "ISIN" });
            DropIndex("ref_holding.INDEX", new[] { "IdentificationId" });
            DropIndex("ref_holding.INDEX", new[] { "Id", "ISIN", "Date" });
            DropIndex("ref_security.EQUITY", new[] { "Id", "ISIN" });
            DropIndex("ref_security.DEBT", new[] { "Id", "ISIN" });
            DropIndex("ref_holding.ASSET_HOLDING", new[] { "AssetId", "AssetISIN" });
            DropIndex("ref_holding.ASSET_HOLDING", new[] { "Id", "ISIN", "Date" });
            DropIndex("ref_security.ASSET_CLASSIFICATION", new[] { "AssetId", "ISINId" });
            DropIndex("ref_security.ASSET", new[] { "RatingId" });
            DropIndex("ref_rating.RATING", new[] { "AssetId", "ISINId" });
            DropIndex("ref_security.PRICE", new[] { "SecurityId", "ISINId" });
            DropIndex("ref_security.ASSET", new[] { "IdentificationId" });
            DropIndex("ref_security.SECURITIES_ISSUANCE", new[] { "SecurityId", "ISINId" });
            DropIndex("ref_security.ASSET_PORTFOLIO", new[] { "Portfolio_Id", "Portfolio_ISINId", "Portfolio_Date" });
            DropIndex("ref_security.ASSET_PORTFOLIO", new[] { "Asset_Id", "Asset_ISINId" });
            DropIndex("ref_holding.VALUATION", new[] { "ContainerId", "ISINId", "ContainerDate" });
            DropIndex("ref_holding.COMPONENT", new[] { "ParentId", "ParentISIN", "ParentDate" });
            DropIndex("ref_issuer.ROLE", new[] { "AssetId", "ISINId" });
            DropTable("ref_holding.PORTFOLIO");
            DropTable("ref_security.FUND");
            DropTable("ref_holding.INDEX");
            DropTable("ref_security.EQUITY");
            DropTable("ref_security.DEBT");
            DropTable("ref_holding.ASSET_HOLDING");
            DropTable("ref_rating.RATING");
            DropTable("ref_security.PRICE");
            DropTable("ref_security.SECURITIES_ISSUANCE");
            DropTable("ref_security.ASSET");
            DropTable("ref_security.ASSET_PORTFOLIO");
            DropTable("ref_common.IDENTIFICATION");
            DropTable("ref_holding.VALUATION");
            DropTable("ref_holding.COMPONENT");
            DropTable("ref_issuer.ROLE");
            DropTable("ref_security.ASSET_CLASSIFICATION");
        }
    }
}
