select i.*,id.* from ref_holding.[INDEX] as i
left outer join ref_common.IDENTIFICATION as id on id.id = i.IdentificationId  
--where i.FamilyKey like '??100%'
--where i.Name like '%Energy%'
where  i.ISIN in ('MXFR','MXEM','MXEUM','MSCI650040','MXEU', 'MXPC','MXEF', 'MXWOM','MSCI_SI')

select * from ref_holding.INDEX_HASHTABLE

select * from ref_holding.[INDEX] as i
left outer join ref_common.IDENTIFICATION as id on id.id = i.IdentificationId  

select * from ref_common.IDENTIFICATION as id 
--where Bloomberg like 'MXEU%'
select * from ref_holding.[INDEX] as i
left outer join ref_common.IDENTIFICATION as id on id.id = i.IdentificationId  
where id.Bloomberg = 'MXUS0MT'
where id.Name like '%EUROPE%Energy%'

where ISIN = 'MSCI703605'
select * from ref_security.PRICE  order by date
select * from ref_holding.ASSET_HOLDING order by ISIN,Date
select * from ref_Holding.VALUATION where IsinId = 'MXEU'  order by date 
select * from ref_security.EQUITY 
select * from ref_security.EQUITY  where FamilyKey like '__100~00~_~000-____________________'
select * from ref_security.ASSET order by FinancialInstrumentName  
select * from ref_security.FX_RATE
select ISINId, COUNT(*) from ref_security.ASSET_CLASSIFICATION 
group by  ISINId 

select * from ref_security.ASSET_CLASSIFICATION 




select v.ValuationSOurce,c.*,i.*,v.* from ref_Holding.VALUATION as v
left outer join ref_holding.[INDEX] as i on i.Id = v.ContainerId
left outer join [ref_holding].COMPONENT as c on c.Id = i.Id 
where v.Date = '31/03/2014'
and i.ISIN in ('MXFR','MXEM','MXEUM','MSCI650040','MXEU', 'MXPC','MXEF', 'MXWOM','MSCI_SI', 'MSCI652525')
order by v.IndexNumberOfSecurities desc

   
-- BEGIN
-- la composition de l indice MXEM
declare @index [nvarchar](12)
--set @index = 'MSCI650040'
set @index = 'MXEM'
declare @dateIndex datetime
set @dateIndex = '01/07/2014'

-- pour le type NextDay, les prix utilisés sont ceux de la veille
declare @datePrice datetime
set @datePrice = '01/07/2014'

declare @indexHoldingType varchar(20)
set @indexHoldingType = 'MSCI_SI'

-- Test si il il y a une zone geographique à prendre en compte
declare @CountryMatrix bit
set @CountryMatrix = 1
IF NOT EXISTS( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = @index and [KEY]= 'CTRY') 
BEGIN
set @CountryMatrix = 0
END

declare @familyKey [nvarchar](45)
set @familyKey = (SELECT familyKey from  ref_holding.[INDEX] where ISIN =@index)


declare @IndexMV float
set @IndexMV = (select v.MarketValue  from  ref_Holding.VALUATION as v where v.Date = @dateIndex and v.ISINId = @index and v.ValuationSource = @indexHoldingType)
set @IndexMV = @IndexMV * 1E6

select @CountryMatrix as CountryHashTable,@familyKey,@IndexMV as marketValue

select Convert(char(10),h.Date,103)as Date, e.ISIN, h.MarketValue/@IndexMV as 'Weight',h.MarketValue,h.Quantity, p.Price,p.Price_Cur,fx.FX,p.Date as 'PriceFX_Date' ,p.Yield_ChangePrice_1D_Value,id.*,e.*, a.*,p.*,h.*
from ref_security.EQUITY as e
left outer join ref_security.ASSET as a on a.Id = e.id
left outer join ref_common.IDENTIFICATION as id on id.id = a.IdentificationId  
left outer join ref_holding.ASSET_HOLDING as h on h.AssetId = e.id and h.Date = @dateIndex and h.ParentISIN = @indexHoldingType
left outer join ref_security.PRICE as p on p.SecurityId = e.id and p.Date = @datePrice
left outer join ref_security.FX_RATE as fx on fx.Date= @datePrice and fx.UnitCurrency = 'USD' and fx.QuotedCurrency = p.Price_Cur and fx.ValuationSource = 'MSCI_SI'
where
h.Id is not null and
h.FamilyKey like  @familyKey   
and ( id.Country in ( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = @index and [KEY]= 'CTRY')
 OR NOT EXISTS( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = @index and [KEY]= 'CTRY') ) 
and ( id.Country in ( select c.ValueCode from ref_holding.INDEX_HASHTABLE as c
                      left outer join ref_holding.INDEX_HASHTABLE as m on m.[KEY] = 'MTRX' and m.ISIN = @index 
                      where c.ISIN = m.ValueCode and c.[KEY]= 'CTRY')
 OR NOT EXISTS( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = @index and [KEY]= 'MTRX') ) 

-- FIN 

select Convert(char(10),v.Date,103)as Date,v.ISINId, v.MarketValue ,v.MarketValue_Cur, v.IndexDivisor, v.IndexNumberOfSecurities, v.IndexPriceValue,v.IndexNetValue,v.IndexGrossValue,v.*  from  ref_Holding.VALUATION as v 
where MONTH(v.Date)= MONTH(@dateIndex) and YEAR(v.Date)= YEAR(@dateIndex) and v.ISINId = @index  and v.ValuationSource = @indexHoldingType
order by v.Date
---FIN

select Convert(char(10),v.Date,103)as Date,v.ISINId, v.MarketValue ,v.MarketValue_Cur, v.IndexDivisor, v.IndexNumberOfSecurities, v.IndexPriceValue,v.IndexNetValue,v.IndexGrossValue,v.*  from  ref_Holding.VALUATION as v 
where v.ISINId = 'MSCI650040' and v.ValuationSource = 'MSCI_SI'
order by v.Date


select Convert(char(10),v.Date,103)as Date,v.ISINId, v.MarketValue ,v.MarketValue_Cur, v.IndexDivisor, v.IndexNumberOfSecurities, v.IndexPriceValue,v.IndexNetValue,v.IndexGrossValue,v.*  from  ref_Holding.VALUATION as v 
where v.ValuationSource = 'MSCI_SI_ND' and ABS( v.Yield_ChangePrice_1D_Value ) > 1E-6
order by v.Date



select * from ref_security.PRICE

select h.*,p.*, id.*,e.*, a.* from ref_security.EQUITY as e
left outer join ref_security.ASSET as a on a.Id = e.id
left outer join ref_security.Price as p on p.SecurityId = e.id 
left outer join ref_common.IDENTIFICATION as id on id.id = a.IdentificationId  
left outer join ref_holding.ASSET_HOLDING as h on h.AssetId = e.id
where h.Date ='31/01/2014'
and e.FamilyKey like 
where e.ISIN ='AT0000730007'
where e.ISIN ='GB0005405286'


select * from ref_holding.[INDEX]

begin transaction
select identificationId 
into #ID
from ref_holding.[INDEX]
 
delete from ref_holding.[INDEX] 
delete from ref_common.IDENTIFICATION where Id in ( select identificationId from #ID)

select ISIN into #EQ 
from ref_security.EQUITY 

delete  from ref_security.EQUITY 
--where ISIN in (select ISIN from #EQ )

delete  from ref_security.Price 
--where ISINId in (select ISIN from #EQ )
delete from ref_Holding.VALUATION 
delete  from ref_security.DEBT  
delete  from ref_security.ASSET 
delete from ref_holding.[ASSET_HOLDING]
delete from ref_holding.[COMPONENT]
--where ISIN in (select ISIN from #EQ )

delete  from ref_common.IDENTIFICATION 
--where ISIN in (select ISIN from #EQ )


delete from ref_security.FX_RATE
delete from ref_security.ASSET_CLASSIFICATION 
--where ISINId  in (select ISIN from #EQ )

--select * from ref_holding.[INDEX]
--select * from ref_common.IDENTIFICATION 
--select * from ref_security.EQUITY 
--select * from ref_security.ASSET 

drop table  #EQ
drop table  #ID


rollback
commit


