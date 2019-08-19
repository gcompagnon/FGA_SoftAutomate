-- COMPOSITION DES INDICES OBLIGATAIRES
-- Possibilité de composer des sosu indices pour former les tranches de maturité plus larges
-- la composition d un indice 

declare @dateIndex datetime
set @dateIndex = '01/09/2014'

declare @ListIndex Table( iBoxxSubIndex [nvarchar](12) )
---- iBoxx EUR Corporates 1-5
insert into @ListIndex values ( 'DE0006301187')--	iBoxx EUR Corporates 1-3
insert into @ListIndex values ( 'DE0006301518')--	iBoxx EUR Corporates 3-5

---- iBoxx EUR France 1-5
--insert into @ListIndex values ( 'DE0009682039' ) -- iBoxx EUR France 1-3
--insert into @ListIndex values ( 'DE0009682070' ) -- iBoxx EUR France 3-5


-- insert into @ListIndex values ( 'DE0001457554') -- EUR CORPORATE SENIOR
--insert into @ListIndex values ( 'DE0009682716' )-- tout IBOXX EUR
--insert into @ListIndex values ( 'DE0006301161' )-- EUR CORPORATE (utilisé par prime obligation
--insert into @ListIndex values ( 'DE000A0ME5U2' )-- iBoxx EUR Corporates 1-10
--insert into @ListIndex values ( 'DE000A0ME5S6' ) -- iBoxx EUR Corporates 10-15

declare @Listing TABLE (IndexDate datetime NOT NULL,IndexCode [nvarchar](12) NOT NULL,ISIN [nvarchar](12) NOT NULL,
	[AssetId] [bigint] NOT NULL,MarketValue float,MarketValueTotal float,MarketValue_Cur [nchar](4),IndexTotal int,
    FaceAmount float,FaceAmount_Cur [nchar](4),BookValue float,BookValue_Cur [nchar](4),[Quantity] [real] NULL,
	[Weight] [float] NULL)

declare @indexI [nvarchar](12)

DECLARE indexes_cursor CURSOR FOR
select iBoxxSubIndex from @ListIndex;

OPEN indexes_cursor;
FETCH NEXT FROM indexes_cursor INTO @indexI;

WHILE @@FETCH_STATUS = 0
   BEGIN
      insert @Listing 
      select * from ref_holding.INDEX_LISTING( @indexI, @dateIndex, 'DE0009682716','IBOXX_EUR')
      FETCH NEXT FROM indexes_cursor INTO @indexI;
   END;

CLOSE indexes_cursor;
DEALLOCATE indexes_cursor;


declare @IndexMV float
set @IndexMV = (select sum(MarketValueTotal/indexTotal) from  @Listing)

select IndexDate,IndexCode,l.ISIN,MarketValue, MarketValue/@IndexMV as 'Weight' , c.Classification5 , rValide.Value, d.FinancialInfos_Seniority_SeniorityLevel
from @Listing as l
left outer join ref_security.ASSET_CLASSIFICATION as c on c.AssetId = l.AssetId and c.Source ='IBOXX_EUR'
left outer join ( select AssetId, MAX(ValueDate) as ValueDate from ref_rating.RATING where RatingScheme ='IBOXX_EUR' and AssetId in (select AssetId from @Listing) and ValueDate <= @dateIndex group by AssetId ) as r on r.AssetId = l.AssetId 
left outer join ref_rating.RATING as rValide on rValide.RatingScheme ='IBOXX_EUR' and rValide.AssetId = l.AssetId  and rValide.ValueDate = r.ValueDate
left outer join ref_security.DEBT as d on d.Id = l.AssetId

GO


---------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------
--- COMPOSANTES 
---------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------
--declare @index [nvarchar](12)
---- set @index = 'DE0001457554' -- EUR CORPORATE SENIOR
--set @index = 'DE0009682716' -- tout IBOXX EUR


--declare @dateIndex datetime
--set @dateIndex = '01/07/2014'

---- Date des prix affiches
---- pour le type NextDay, les prix utilisés sont ceux de la veille
--declare @datePrice datetime
--set @datePrice = @dateIndex


--declare @indexHoldingType varchar(20)
--set @indexHoldingType = 'IBOXX_EUR'

---- l identifiant de l indice global (le plus gros ensemble)
---- NE PAS TOUCHER
--declare @indexAssetHolding [nvarchar](12)
--set @indexAssetHolding = 'DE0009682716'


---- Test si il il y a une zone geographique à prendre en compte
--declare @CountryMatrix bit
--set @CountryMatrix = 1
--IF NOT EXISTS( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = @index and [KEY]= 'CTRY') 
--BEGIN
--set @CountryMatrix = 0
--END

--declare @familyKey [nvarchar](45)
--set @familyKey = (SELECT familyKey from  ref_holding.[INDEX] where ISIN =@index)

--declare @IndexMV float
--set @IndexMV = (select v.MarketValue  from  ref_Holding.VALUATION as v where v.Date = @dateIndex and v.ISINId = @index and v.ValuationSource = @indexHoldingType)
----set @IndexMV = @IndexMV * 1E6


--select Convert(char(10),v.Date,103)as Date,@familyKey, @IndexMV,
--v.ISINId,i.Name, v.MarketValue ,v.MarketValue_Cur,
--@CountryMatrix as CountryHashTableFlag,@familyKey as IndexFamilyKey,
--v.IndexDivisor, v.IndexNumberOfSecurities, v.IndexPriceValue,v.IndexNetValue,v.IndexGrossValue,v.* 
--from  ref_Holding.VALUATION as v 
--left outer join ref_holding.[INDEX] as i on i.Id = v.containerId
--where v.Date=@dateIndex and v.ISINId = @index  and v.ValuationSource = @indexHoldingType
--and ValuationSource = @indexHoldingType
--order by v.ValuationSource,v.Date


--select Convert(char(10),h.Date,103)as Date, e.ISIN, h.MarketValue/@IndexMV as 'Weight',h.MarketValue,h.Quantity, p.Price,p.Price_Cur,
--fx.FX,p.Date as 'PriceFX_Date' ,
--r.*,
--p.Yield_ChangePrice_1D_Value,id.*,e.*, a.*,p.*,h.*
--from ref_security.DEBT as e
--left outer join ref_security.ASSET as a on a.Id = e.id
--left outer join ref_common.IDENTIFICATION as id on id.id = a.IdentificationId  
--left outer join ref_holding.ASSET_HOLDING as h on h.AssetId = e.id and h.Date = @dateIndex and h.ParentISIN = @indexAssetHolding
--left outer join ref_security.PRICE as p on p.SecurityId = e.id and p.Date = @datePrice and p.Price_Source = @indexHoldingType
--left outer join ref_rating.RATING as r on r.AssetId = e.id and r.RatingScheme =@indexHoldingType and r.ValueDate < @datePrice
--left outer join ref_security.FX_RATE as fx on fx.Date= @datePrice and fx.UnitCurrency = 'USD' and fx.QuotedCurrency = p.Price_Cur and fx.ValuationSource = 'MSCI_SI'
--where
--h.Id is not null and
--h.FamilyKey like  @familyKey   
--and ( id.Country in ( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = @index and [KEY]= 'CTRY')
-- OR NOT EXISTS( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = @index and [KEY]= 'CTRY') ) 
--and ( id.Country in ( select c.ValueCode from ref_holding.INDEX_HASHTABLE as c
--                      left outer join ref_holding.INDEX_HASHTABLE as m on m.[KEY] = 'MTRX' and m.ISIN = @index 
--                      where c.ISIN = m.ValueCode and c.[KEY]= 'CTRY')
-- OR NOT EXISTS( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = @index and [KEY]= 'MTRX') ) 

---- FIN 
