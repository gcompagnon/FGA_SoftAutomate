----------------------
-- extraction de Barclays returns pour JUMP 
-- version beta pour aller vite

--  pour recuperer les tables de transco pour les type valeurs Omegae
--CREATE TABLE #JUMP_OMEGA_TYPE (	type_produit nvarchar(50) NOT NULL,	type_jump nchar(8) NULL )
--  select *, 'INSERT INTO #JUMP_OMEGA_TYPE VALUES ('''+RTRIM(type_produit)+''','''+RTRIM(type_jump)+''')' from JUMP_OMEGA_TYPE jot



-- extraction pour JUMP pour le reporting

declare @date datetime
set @date = '29/05/2015'

declare @Indice as varchar(24)
set  @Indice = 'INDEX_288'

declare @IndiceA as varchar(24)
set  @IndiceA = 'INDEX_00288'

declare @IndiceISIN as varchar(24)
set  @IndiceISIN = 'BT5ATREU'  --Euro Treasury 5-7 Yr Custom A and Above


--select c.ParentISIN, h.ISIN,h.MarketValue,h.* from [ref_holding].ASSET_HOLDING as h
--left outer join [ref_holding].COMPONENT as c on c.Id = h.Id
--where h.Date = @date and c.ParentISIN = @IndiceISIN
--order by h.ISIN 

select  c.ParentISIN, h.Date, sum(h.MarketValue) as MV--, v.MarketValue  
into #MARKETVALUETOTAL
from [ref_holding].ASSET_HOLDING as h
left outer join [ref_holding].COMPONENT as c on c.Id = h.Id
--left outer join FGA_DATAMODEL.ref_holding.VALUATION as v on v.ContainerId = c.ParentId and v.Date = h.Date
where c.ParentISIN = @IndiceISIN
group by c.ParentISIN, h.Date--,v.MarketValue
order by ParentISIN,Date
 
  
select  * from #MARKETVALUETOTAL
select 
	@date                 as [date], 
	@Indice               as [name],
	'FEDERIS'             as [portfolio source],
	@Indice	              as [portfolio code],
	'00000'               as [bank code],
	'00000'               as [branch code],
	@IndiceA              as [account],
	'EUR'                 as [base currency],
	'BOND'                as [type],
	a.FinancialInstrumentName as [asset],
	h.MarketValue_Cur     as [currency],
	'no_place'            as [market place],
	999                   as [telekurs code],
	a.ISIN                as [isin code],
	id.CUSIP              as [cusip code],
	id.Ticker             as [Bloomberg code],
	''                    as [Beauchamp code],
	''                    as [Local Code],		
	1                     as [position],
	h.MarketValue / total.MV              as [base amount],
	h.MarketValue / total.MV              as [average absorption cost],
	0                     as [base PL],
	1                     as [change rate],
	0                     as [line PL],	
	h.MarketValue / total.MV              as [line amount]	

from [ref_holding].ASSET_HOLDING as h
--left outer join ref_holding.[INDEX] as i on i.ISIN = h.ParentISIN
left outer join [ref_security].ASSET as a on a.Id = h.AssetId
left outer join [ref_common].IDENTIFICATION as id on id.Id = a.IdentificationId
left outer join #MARKETVALUETOTAL as total on total.ParentISIN = h.ParentISIN and total.Date = h.Date
where h.Date= @date
and h.ParentISIN = @IndiceISIN


--  select * from [FGA_DATAMODEL].[ref_common].IDENTIFICATION

drop table #MARKETVALUETOTAL

--  select distinct h.DATE, h.ParentISIN,i.Name, count(*) from [FGA_DATAMODEL].[ref_holding].ASSET_HOLDING as h
--left outer join FGA_DATAMODEL.ref_holding.[INDEX] as i on i.ISIN = h.ParentISIN
--group by h.DATE, ParentISIN,i.Name
--order by h.Date desc

--select * from FGA_DATAMODEL.ref_holding.[INDEX]
--select c.ParentISIN, sum(h.MarketValue) from [FGA_DATAMODEL].[ref_holding].ASSET_HOLDING as h
--left outer join [FGA_DATAMODEL].[ref_holding].COMPONENT as c on c.Id = h.Id
--where h.Date = @date and h.ParentISIN = @IndiceISIN
--group by c.ParentISIN

