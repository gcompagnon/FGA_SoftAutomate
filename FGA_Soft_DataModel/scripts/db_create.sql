
if (schema_id(N'ref_common') is null) exec(N'create schema [ref_common]');
if (schema_id(N'ref_holding') is null) exec(N'create schema [ref_holding]');
if (schema_id(N'ref_issuer') is null) exec(N'create schema [ref_issuer]');
if (schema_id(N'ref_rating') is null) exec(N'create schema [ref_rating]');
if (schema_id(N'ref_security') is null) exec(N'create schema [ref_security]');
create table [ref_security].[ASSET] (
    [Id] [bigint] not null identity,
    [ISIN] [nchar](12) not null,
    [FinancialInstrumentName] [nvarchar](350) null,
    [MaturityDate] [datetime] null,
    [FinancialAssetCategory_FinancialAssetTypeCategory] [char](1) null,
    [IdentificationId] [bigint] not null,
    [RatingId] [bigint] null,
    [Discriminator] [nvarchar](128) not null,
    primary key ([Id], [ISIN])
);
create table [ref_holding].[ASSET_HOLDING] (
    [Id] [bigint] not null,
    [ISIN] [nchar](12) not null,
    [Date] [datetime] not null,
    [ParentISIN] [nchar](12) null,
    [AssetId] [bigint] not null,
    [AssetISIN] [nchar](12) not null,
    [FaceAmount] [float] null,
    [FaceAmount_Cur] [nchar](3) null,
    [MarketValue] [float] null,
    [MarketValue_Cur] [nchar](3) null,
    [BookValue] [float] null,
    [BookValue_Cur] [nchar](3) null,
    [Quantity] [real] null,
    primary key ([Id], [ISIN], [Date])
);
create table [ref_security].[ASSET_CLASSIFICATION] (
    [Id] [bigint] not null identity,
    [Source] [nvarchar](128) not null,
    [Classification1] [nvarchar](max) null,
    [Classification2] [nvarchar](max) null,
    [Classification3] [nvarchar](max) null,
    [Classification4] [nvarchar](max) null,
    [Classification5] [nvarchar](max) null,
    [Classification6] [nvarchar](max) null,
    [Classification7] [nvarchar](max) null,
    [AssetId] [bigint] not null,
    [ISINId] [nchar](12) not null,
    primary key ([Id], [Source])
);
create table [ref_security].[ASSET_PORTFOLIO] (
    [Id] [bigint] not null identity,
    [InvestmentAmount] [float] null,
    [InvestmentAmount_Cur] [nchar](3) null,
    [InvestmentRate] [float] null,
    [EffectiveDate] [datetime] null,
    [Portfolio_Id] [bigint] null,
    [Portfolio_ISINId] [nchar](12) null,
    [Portfolio_Date] [datetime] null,
    [Asset_Id] [bigint] null,
    [Asset_ISINId] [nchar](12) null,
    primary key ([Id])
);
create table [ref_security].[SECURITIES_ISSUANCE] (
    [SecurityId] [bigint] null,
    [Id] [bigint] not null identity,
    [Date] [datetime] null,
    [Unit] [float] null,
    [Amount_Value] [float] null,
    [Amount_Currency_Currency] [nchar](3) null,
    [Type_CapitalType] [nvarchar](max) null,
    [ISINId] [nchar](12) null,
    primary key ([Id])
);
create table [ref_holding].[COMPONENT] (
    [Id] [bigint] not null identity,
    [ISIN] [nchar](12) not null,
    [Date] [datetime] not null,
    [ParentId] [bigint] null,
    [ParentISIN] [nchar](12) null,
    [ParentDate] [datetime] null,
    primary key ([Id], [ISIN], [Date])
);
create table [ref_security].[DEBT] (
    [Id] [bigint] not null,
    [ISIN] [nchar](12) not null,
    [NextInterest_CalcFreq] [char](1) null,
    [NextInterest_Rate] [float] null,
    [NextInterest_DayCountBasis_InterestComputationMethod] [nvarchar](max) null,
    [FirstPaymentDate] [datetime] null,
    [PutableDate] [datetime] null,
    [PutableIndicator] [bit] null,
    [SinkableIndicator] [bit] null,
    [FixedToVariableIndicator] [bit] null,
    [VariableRateIndicator] [bit] null,
    [NextCallableDate] [datetime] null,
    [PerpetualIndicator] [bit] null,
    [FinancialInfos_Seniority_SeniorityLevel] [char](12) null,
    primary key ([Id], [ISIN])
);
create table [ref_security].[EQUITY] (
    [Id] [bigint] not null,
    [ISIN] [nchar](12) not null,
    primary key ([Id], [ISIN])
);
create table [ref_security].[FUND] (
    [Id] [bigint] not null,
    [ISIN] [nchar](12) not null,
    [DistributionPolicy_DistributionPolicy] [char](1) null,
    [DividendPolicy_DividendPolicy] [char](1) null,
    primary key ([Id], [ISIN])
);
create table [ref_holding].[INDEX] (
    [Id] [bigint] not null,
    [ISIN] [nchar](12) not null,
    [Date] [datetime] not null,
    [Name] [nvarchar](350) null,
    [IdentificationId] [bigint] not null,
    [IndexFixingDate] [datetime] null,
    [IndexFrequency] [char](1) null,
    [IndexCurrency] [nchar](3) null,
    primary key ([Id], [ISIN], [Date])
);
create table [ref_holding].[PORTFOLIO] (
    [Id] [bigint] not null,
    [ISIN] [nchar](12) not null,
    [Date] [datetime] not null,
    [Name] [nvarchar](350) null,
    [IdentificationId] [bigint] not null,
    primary key ([Id], [ISIN], [Date])
);
create table [ref_rating].[RATING] (
    [Id] [bigint] not null identity,
    [ISINId] [nchar](12) not null,
    [AssetId] [bigint] not null,
    [Value] [nvarchar](max) null,
    [RatingScheme] [nvarchar](max) null,
    [ValueDate] [datetime] null,
    [Moody] [nvarchar](max) null,
    [MoodyDate] [datetime] null,
    [SnP] [nvarchar](max) null,
    [SnPDate] [datetime] null,
    [Fitch] [nvarchar](max) null,
    [FitchDate] [datetime] null,
    primary key ([Id])
);
create table [ref_issuer].[ROLE] (
    [Id] [bigint] not null identity,
    [AssetId] [bigint] not null,
    [ISINId] [nchar](12) not null,
    [Country] [nchar](2) null,
    [IssuerName] [nvarchar](60) null,
    [Discriminator] [nvarchar](128) not null,
    primary key ([Id])
);
create table [ref_common].[IDENTIFICATION] (
    [Id] [bigint] not null identity,
    [Name] [nvarchar](350) null,
    [ISIN] [nchar](12) null,
    [Country] [nchar](2) null,
    [OtherIdentification] [nvarchar](35) null,
    [ProprietaryIdentificationSource] [nvarchar](35) null,
    [Bloomberg] [nchar](20) null,
    [Reuters] [nchar](20) null,
    [SEDOL] [nchar](20) null,
    [CUSIP] [nchar](20) null,
    [Ticker] [nchar](20) null,
    [TradingIdentification] [nvarchar](70) null,
    primary key ([Id])
);
create table [ref_security].[PRICE] (
    [Id] [bigint] not null identity,
    [Date] [datetime] not null,
    [Price] [float] null,
    [Price_Cur] [nchar](3) null,
    [Price_Type] [nvarchar](max) null,
    [PriceSource] [nvarchar](max) null,
    [Price_Ask] [float] null,
    [Price_Bid] [float] null,
    [Price_Mid] [float] null,
    [PriceFactType_LastPrice] [float] null,
    [PriceFactType_BestPrice] [float] null,
    [PriceFactType_ClosePrice] [float] null,
    [PriceFactType_OpenPrice] [float] null,
    [PriceFactType_LowPrice] [float] null,
    [PriceFactType_HighPrice] [float] null,
    [Yield_ChangePrice_1D_Value] [float] null,
    [Yield_ChangePrice_MTD_Value] [float] null,
    [Yield_ChangePrice_YTD_Value] [float] null,
    [Debt_CleanP] [float] null,
    [Debt_DirtyP] [float] null,
    [Debt_AI] [float] null,
    [Debt_Duration] [float] null,
    [Debt_Sensitivity] [float] null,
    [Debt_TTM] [float] null,
    [Debt_Convexity] [float] null,
    [DebtDataCalculation_MacaulayDurationSemiAnnual] [float] null,
    [DebtDataCalculation_ModifiedDurationSemiAnnual] [float] null,
    [DebtDataCalculation_ConvexitySemiAnnual] [float] null,
    [DebtDataCalculation_WeightedAverageLife] [float] null,
    [Debt_YTM_Rate] [float] null,
    [DebtYield_YieldToWorstRate_Value] [float] null,
    [DebtYield_YieldToNextCallRate_Value] [float] null,
    [DebtSpread_GovSpread] [float] null,
    [DebtSpread_InterpolatedSwapSpread] [float] null,
    [DebtSpread_CDSBondBasisSpread] [float] null,
    [DebtSpread_ZeroVolatilitySpread] [float] null,
    [DebtSpread_AssetSwapSpread] [float] null,
    [DebtSpread_OptionAdjustedSpread] [float] null,
    [SecurityId] [bigint] null,
    [ISINId] [nchar](12) null,
    primary key ([Id], [Date])
);
create table [ref_holding].[VALUATION] (
    [Id] [bigint] not null identity,
    [Date] [datetime] not null,
    [FaceAmount] [float] null,
    [FaceAmount_Cur] [nchar](3) null,
    [MarketValue] [float] null,
    [MarketValue_Cur] [nchar](3) null,
    [BookValue] [float] null,
    [BookValue_Cur] [nchar](3) null,
    [Debt_Duration] [float] null,
    [Debt_Sensitivity] [float] null,
    [Debt_TTM] [float] null,
    [Debt_Convexity] [float] null,
    [DebtDataCalculation_MacaulayDurationSemiAnnual] [float] null,
    [DebtDataCalculation_ModifiedDurationSemiAnnual] [float] null,
    [DebtDataCalculation_ConvexitySemiAnnual] [float] null,
    [DebtDataCalculation_WeightedAverageLife] [float] null,
    [Debt_YTM_Rate] [float] null,
    [DebtYield_YieldToWorstRate_Value] [float] null,
    [DebtYield_YieldToNextCallRate_Value] [float] null,
    [DebtSpread_GovSpread] [float] null,
    [DebtSpread_InterpolatedSwapSpread] [float] null,
    [DebtSpread_CDSBondBasisSpread] [float] null,
    [DebtSpread_ZeroVolatilitySpread] [float] null,
    [DebtSpread_AssetSwapSpread] [float] null,
    [DebtSpread_OptionAdjustedSpread] [float] null,
    [ContainerId] [bigint] null,
    [ISINId] [nchar](12) null,
    [ContainerDate] [datetime] not null,
    primary key ([Id], [Date])
);
alter table [ref_security].[ASSET_CLASSIFICATION] add constraint [AssetClassification_Asset] foreign key ([AssetId], [ISINId]) references [ref_security].[ASSET]([Id], [ISIN]) on delete cascade;
alter table [ref_holding].[ASSET_HOLDING] add constraint [AssetHolding_Asset] foreign key ([AssetId], [AssetISIN]) references [ref_security].[ASSET]([Id], [ISIN]) on delete cascade;
alter table [ref_holding].[ASSET_HOLDING] add constraint [AssetHolding_TypeConstraint_From_Component_To_ASSET_HOLDING] foreign key ([Id], [ISIN], [Date]) references [ref_holding].[COMPONENT]([Id], [ISIN], [Date]);
alter table [ref_security].[ASSET_PORTFOLIO] add constraint [AssetPortfolioAssociation_Asset] foreign key ([Asset_Id], [Asset_ISINId]) references [ref_security].[ASSET]([Id], [ISIN]);
alter table [ref_security].[ASSET_PORTFOLIO] add constraint [AssetPortfolioAssociation_Portfolio] foreign key ([Portfolio_Id], [Portfolio_ISINId], [Portfolio_Date]) references [ref_holding].[PORTFOLIO]([Id], [ISIN], [Date]);
alter table [ref_security].[SECURITIES_ISSUANCE] add constraint [Capital_SecuritiesIssuance] foreign key ([SecurityId], [ISINId]) references [ref_security].[ASSET]([Id], [ISIN]);
alter table [ref_holding].[COMPONENT] add constraint [Component_Parent] foreign key ([ParentId], [ParentISIN], [ParentDate]) references [ref_holding].[COMPONENT]([Id], [ISIN], [Date]);
alter table [ref_security].[DEBT] add constraint [Debt_TypeConstraint_From_Asset_To_DEBT] foreign key ([Id], [ISIN]) references [ref_security].[ASSET]([Id], [ISIN]);
alter table [ref_security].[EQUITY] add constraint [Equity_TypeConstraint_From_Asset_To_EQUITY] foreign key ([Id], [ISIN]) references [ref_security].[ASSET]([Id], [ISIN]);
alter table [ref_holding].[INDEX] add constraint [Index_Identification1] foreign key ([IdentificationId]) references [ref_common].[IDENTIFICATION]([Id]) on delete cascade;
alter table [ref_holding].[INDEX] add constraint [Index_TypeConstraint_From_Component_To_INDEX] foreign key ([Id], [ISIN], [Date]) references [ref_holding].[COMPONENT]([Id], [ISIN], [Date]);
alter table [ref_security].[FUND] add constraint [InvestmentFund_TypeConstraint_From_Asset_To_FUND] foreign key ([Id], [ISIN]) references [ref_security].[ASSET]([Id], [ISIN]);
alter table [ref_holding].[PORTFOLIO] add constraint [Portfolio_Identification1] foreign key ([IdentificationId]) references [ref_common].[IDENTIFICATION]([Id]) on delete cascade;
alter table [ref_holding].[PORTFOLIO] add constraint [Portfolio_TypeConstraint_From_Component_To_PORTFOLIO] foreign key ([Id], [ISIN], [Date]) references [ref_holding].[COMPONENT]([Id], [ISIN], [Date]);
alter table [ref_rating].[RATING] add constraint [Rating_RatedSecurity] foreign key ([AssetId], [ISINId]) references [ref_security].[ASSET]([Id], [ISIN]) on delete cascade;
alter table [ref_issuer].[ROLE] add constraint [Role_Asset] foreign key ([AssetId], [ISINId]) references [ref_security].[ASSET]([Id], [ISIN]) on delete cascade;
alter table [ref_security].[PRICE] add constraint [SecuritiesPricing_PricedSecurity] foreign key ([SecurityId], [ISINId]) references [ref_security].[ASSET]([Id], [ISIN]);
alter table [ref_security].[ASSET] add constraint [Security_Identification] foreign key ([IdentificationId]) references [ref_common].[IDENTIFICATION]([Id]) on delete cascade;
alter table [ref_security].[ASSET] add constraint [Security_Rating] foreign key ([RatingId]) references [ref_rating].[RATING]([Id]);
alter table [ref_holding].[VALUATION] add constraint [Valuation_Valuated] foreign key ([ContainerId], [ISINId], [ContainerDate]) references [ref_holding].[COMPONENT]([Id], [ISIN], [Date]);

---
CREATE NONCLUSTERED INDEX ASSET_HOLDING_AssetId_Index1 ON [ref_holding].ASSET_HOLDING     (  [AssetId] ASC)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, 
DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX PRICE_SecurityId_Index1 ON [ref_security].[PRICE]     (  [SecurityId] ASC)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, 
DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]
GO