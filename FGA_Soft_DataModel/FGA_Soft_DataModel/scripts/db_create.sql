02-01-2012-12-53-00
if (schema_id(N'common') is null) exec(N'create schema [common]');
if (schema_id(N'index') is null) exec(N'create schema [index]');
if (schema_id(N'ref') is null) exec(N'create schema [ref]');
create table [ref].[ASSET_CLASSIFICATION] (
    [ClassificationId] [int] not null identity,
    [IndexAsset_BasketId] [int] null,
    primary key ([ClassificationId])
);
create table [dbo].[EdmMetadata] (
    [Id] [int] not null identity,
    [ModelHash] [nvarchar](max) null,
    primary key ([Id])
);
create table [index].[INDEX_DEF] (
    [IndexId] [int] not null identity,
    [IndexFixingDate] [datetime] not null,
    [IndexFrequency_IndexFrequency] [nvarchar](1) null,
    [IndexCurrency_Currency] [nvarchar](3) null,
    [IndexRateBasis_Value] [float] null,
    [IndexRateCurrency_Currency] [nvarchar](3) null,
    [IndexRateFrequency_IndexFrequency] [nvarchar](1) null,
    [Identification_SecurityIdentification] [nvarchar](12) null,
    [IndexType] [nvarchar](128) not null,
    primary key ([IndexId])
);
create table [index].[INDEX_ASSET] (
    [BasketId] [int] not null identity,
    [IndexId] [int] not null,
    [Description] [nvarchar](350) null,
    [EffectiveDate] [datetime] not null,
    [MarketCapitalization_Value] [float] not null,
    [MarketCapitalization_Currency_Currency] [nvarchar](3) null,
    [InvestmentRate_Value] [float] not null,
    [HeldNumber_Rate_Value] [float] not null,
    [HeldNumber_Unit] [int] not null,
    [HeldNumber_Amount_Value] [float] not null,
    [HeldNumber_Amount_Currency_Currency] [nvarchar](3) null,
    [ISIN] [nvarchar](12) null,
    primary key ([BasketId])
);
create table [common].[IDENTIFICATION] (
    [ISIN] [nvarchar](12) not null,
    [Country] [nvarchar](2) null,
    [OtherIdentification] [nvarchar](35) null,
    [ProprietaryIdentificationSource] [nvarchar](35) null,
    [BBCode] [nvarchar](20) null,
    [RIC_RICCode] [nvarchar](20) null,
    [SEDOL_SEDOL] [nvarchar](20) null,
    primary key ([ISIN])
);
alter table [index].[INDEX_DEF] add constraint [Index_Identification] foreign key ([Identification_SecurityIdentification]) references [common].[IDENTIFICATION]([ISIN]);
alter table [ref].[ASSET_CLASSIFICATION] add constraint [IndexAsset_AssetClassification] foreign key ([IndexAsset_BasketId]) references [index].[INDEX_ASSET]([BasketId]);
alter table [index].[INDEX_ASSET] add constraint [IndexBasket_Compo] foreign key ([IndexId]) references [index].[INDEX_DEF]([IndexId]) on delete cascade;
