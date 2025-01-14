
SELECT a.date,a.ParentIsin,SUM( a.Weight) as Weight, SUM(a.MarketValue) as MV_Calcul,
 v.MarketValue as MV,
SUM(a.FaceAmount)as FaceAmount_Calcul,v.FaceAmount
  FROM [FGA_DATAMODEL].[ref_holding].[ASSET_HOLDING] as a
  left outer join   [FGA_DATAMODEL].[ref_holding].[VALUATION] as v on v.ISINId = a.ParentISIN and v.Date = a.Date
  group by a.Date,a.ParentISIN, v.MarketValue,v.FaceAmount
  order by a.ParentISIN
  
  select 1000*a.Quantity*(p.Debt_CleanP+p.Debt_AI)/100,a.MarketValue, p.*,a.* FROM [FGA_DATAMODEL].[ref_holding].[ASSET_HOLDING] as a
  left outer join [FGA_DATAMODEL].[ref_security].PRICE as p on p.SecurityId = a.AssetId and p.Date = a.Date  
  where a.ParentISIN = 'DE0009682716'
  order by ISIN,a.Date
  
  
   select * FROM [FGA_DATAMODEL].[ref_holding].[ASSET_HOLDING] as a where a.ParentISIN = 'GB00B5SLB066'