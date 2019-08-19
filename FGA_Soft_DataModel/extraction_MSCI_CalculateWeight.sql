
-- Calcul du MarketCap de chaque indice
-- 
declare @dateIndex datetime
set @dateIndex = '01/07/2014'

declare @indexHoldingType varchar(20)
set @indexHoldingType = 'MSCI_SI'

select Convert(char(10),@dateIndex,103)as Date,i.ISIN as indice, h.MarketValue as 'MarketValue',i.Name
 
into #MARKETCAP
from [ref_holding].[INDEX] as i
left outer join ref_security.EQUITY as e on e.Id in ( select AssetId from ref_holding.ASSET_HOLDING where Date = @dateIndex and ParentISIN = @indexHoldingType and Id is not null)  
left outer join ref_security.ASSET as a on a.Id = e.id
left outer join ref_common.IDENTIFICATION as id on id.id = a.IdentificationId  
left outer join ref_holding.ASSET_HOLDING as h on h.Date = @dateIndex and h.ParentISIN = @indexHoldingType and h.AssetId = e.Id  
where i.FamilyKey is not null
and h.FamilyKey like  i.FamilyKey
and  i.Name not like '%GROWTH%'
and i.Name not like '%VALUE%'
and i.Name not like '%ISLAMIC%'
and i.FamilyKey not like '%~%S%~%' -- pas de small cap
and i.ISIN not in ('MSCI703605','MSCI703606')
and ( id.Country in ( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = i.ISIN and [KEY]= 'CTRY')
 OR NOT EXISTS( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = i.ISIN and [KEY]= 'CTRY') ) 
and ( id.Country in ( select c.ValueCode from ref_holding.INDEX_HASHTABLE as c
                      left outer join ref_holding.INDEX_HASHTABLE as m on m.[KEY] = 'MTRX' and m.ISIN = i.ISIN 
                      where c.ISIN = m.ValueCode and c.[KEY]= 'CTRY')
 OR NOT EXISTS( select ValueCode from ref_holding.INDEX_HASHTABLE where ISIN = i.ISIN and [KEY]= 'MTRX') ) 


select DATE, rtrim(indice) as ISIN, Name,SUM(MarketValue)/1E6 as MarketValue,  COUNT(*) as NbSec
from #MARKETCAP
group by Date,indice,Name

----- les compositions des indices disponibles
--select * from [ref_holding].[INDEX] as i 
--where i.Name not like '%GROWTH%'
--and i.Name not like '%VALUE%'
--and i.Name not like '%ISLAMIC%'
-- and i.FamilyKey not like '_____-__-%S%-%'
-- and i.ISIN not in ('MSCI703605','MSCI703606')


 
drop table #MARKETCAP