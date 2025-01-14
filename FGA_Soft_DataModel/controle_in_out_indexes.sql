 -- les sorties et entrée des indices 
 create table #INDICES_PARAM (ParentISIN nchar(12) ) 
 
 -- Barclays
 --insert into #INDICES_PARAM values ('BTS1TREU')
 --insert into #INDICES_PARAM values ('BT5ATREU')
 --insert into #INDICES_PARAM values ('BT11TREU')
 
 --insert into #INDICES_PARAM values ('DE0009682716')  -- Iboxx Overall
 --insert into #INDICES_PARAM values ('DE0006301161')  -- Iboxx Corporate
 
 -- MSCI
 insert into #INDICES_PARAM values ('MSCI_SI')
 insert into #INDICES_PARAM values ('MSCI_SI_ND')

 
 
 declare @dateIn DATETIME ='01/04/2014' -- 1er jour du mois pour les entrees
 declare @dateOut DATETIME ='01/03/2014' -- 1er jour du mois pour les sorties d indice
  
---------------------------------------------------------------------------
---------------------------------------------------------------------------
 declare @dateInEOM DATETIME 
 set @dateInEOM = ( select DATEADD(DD, -1, DATEADD(MM, +1, @dateIn) ) ) -- recuperer la fin de mois du dateIn
 
 declare @dateOutEOM DATETIME
 set @dateOutEOM = ( select DATEADD(DD, -1, DATEADD(MM, +1, @dateOut) ) )

 select @dateIn,@dateInEOM
 

 
 select distinct 'OUT ' + CONVERT(varchar,@dateOutEOM,103) as Sens, h.ParentISIN,i.Name,
  --h.Date,
   h.ISIN,d.FinancialInstrumentName
from [FGA_DATAMODEL].[ref_holding].ASSET_HOLDING as h
left outer join [FGA_DATAMODEL].ref_holding.[INDEX] as i on i.ISIN = h.ParentISIN
left outer join [FGA_DATAMODEL].ref_security.ASSET as d on d.Id = h.AssetId
where h.ParentISIN in (select ParentISIN from #INDICES_PARAM)
and h.Date between @dateOut and @dateOutEOM
and h.ISIN not in 
(
 select distinct m.ISIN
from [FGA_DATAMODEL].[ref_holding].ASSET_HOLDING as m
where m.ParentISIN in (select ParentISIN from #INDICES_PARAM)
and m.Date between @dateIn and @dateInEOM
)

UNION
 
 select distinct 'IN ' + CONVERT(varchar,@dateIn,103) as Sens, h.ParentISIN,i.Name,
  --h.Date,
   h.ISIN,d.FinancialInstrumentName
from [FGA_DATAMODEL].[ref_holding].ASSET_HOLDING as h
left outer join [FGA_DATAMODEL].ref_holding.[INDEX] as i on i.ISIN = h.ParentISIN
left outer join [FGA_DATAMODEL].ref_security.ASSET as d on d.Id = h.AssetId
where h.ParentISIN in (select ParentISIN from #INDICES_PARAM)
and h.Date between @dateIn and @dateInEOM
and h.ISIN not in 
(
 select distinct m.ISIN
from [FGA_DATAMODEL].[ref_holding].ASSET_HOLDING as m
where m.ParentISIN in (select ParentISIN from #INDICES_PARAM)
and m.Date between @dateOut and @dateOutEOM
)
order by sens ,ParentISIN, i.Name

drop table #INDICES_PARAM
