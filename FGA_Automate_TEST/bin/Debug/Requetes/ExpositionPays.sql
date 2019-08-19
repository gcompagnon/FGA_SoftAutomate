
declare @date as datetime
SET @date = '31/05/2012'

declare @rapportCle as char(20)
set @rapportCle='MonitorExpoPays'

declare @cle_Montant as char(20)	
set @cle_Montant='MonitorExpoPays'

declare @rubriqueEncours as char(20)
set @rubriqueEncours='TOTAL' 

declare @sousRubriqueEncours as char(20)
set @sousRubriqueEncours='TOTAL_HOLDING' 

select
case when (100*gr1.classementRubrique +10*gr1.classementSousRubrique)=0 then 1 
		when (100*gr1.classementRubrique +10*gr1.classementSousRubrique)/10000=CONVERT(INT,(100*gr1.classementRubrique +10*gr1.classementSousRubrique)/10000) then 0
		when (100*gr1.classementRubrique +10*gr1.classementSousRubrique)/100=CONVERT(INT,(100*gr1.classementRubrique +10*gr1.classementSousRubrique)/100) then 2
                when gr1.classementRubrique/10=CONVERT(INT,gr1.classementRubrique/10) then 3
                else 4
		end as 'style',
case when gr0.classementRubrique=0 then ''
		when (100*gr1.classementRubrique +10*gr1.classementSousRubrique)/10000=CONVERT(INT,(100*gr1.classementRubrique +10*gr1.classementSousRubrique)/10000) then 'graph1'
		else ''
		end as 'graph',
   100*gr1.classementRubrique +10*gr1.classementSousRubrique
   as 'ordre',  
   gr1.classementRubrique, gr1.classementSousRubrique, 
   gr1.rubrique,gr1.sousRubrique, gr1.libelle, 
   gr1.valeur as 'MM AGIRC' ,
   gr2.valeur as 'MM ARRCO',  
   gr3.valeur as 'RETRAITE',  
   (gr3_amount.valeur/NULLIF(gr3_all.valeur,0)) as ' '
  from PTF_RAPPORT_NIV2 as gr0
  left outer join PTF_RAPPORT_NIV2 as gr1 on gr1.rubrique = gr0.rubrique and gr1.sousRubrique = gr0.sousRubrique 
   and gr1.groupe= 'MM AGIRC' and gr1.cle = @rapportCle and gr1.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr2 on gr2.rubrique = gr0.rubrique and gr2.sousRubrique = gr0.sousRubrique 
   and gr2.Groupe= 'MM ARRCO' and gr2.cle = @rapportCle and gr2.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr3 on gr3.rubrique = gr0.rubrique and gr3.sousRubrique = gr0.sousRubrique 
   and gr3.Groupe= 'RETRAITE' and gr3.cle = @rapportCle and gr3.date = @date   
   
  left outer join PTF_RAPPORT_NIV2 as gr3_amount on gr3_amount.rubrique = gr0.rubrique and gr3_amount.sousRubrique = gr0.sousRubrique 
   and gr3_amount.Groupe= gr3.Groupe and gr3_amount.cle = @cle_Montant and gr3_amount.date = @date      
  left outer join PTF_RAPPORT_NIV2 as gr3_all on gr3_all.rubrique = @rubriqueEncours and gr3_all.sousRubrique = @sousRubriqueEncours 
   and gr3_all.Groupe= gr3.Groupe and gr3_all.cle = @cle_Montant and gr3_all.date = @date      
   
      where gr0.Groupe= 'FGA' and gr0.cle = @rapportCle and gr0.date = @date      
  order by gr0.classementRubrique, gr0.classementSousRubrique


select  
case when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)=0 then 1 
		when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000) then 0
		when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100) then 2
                when gr0.classementRubrique/10=CONVERT(INT,gr0.classementRubrique/10) then 3
                else 4
		end as 'style',
case when gr0.classementRubrique=0 then ''
		when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000) then 'graph1'
		else ''
		end as 'graph',
   100*gr0.classementRubrique +10*gr0.classementSousRubrique
   as 'ordre',  
  gr0.classementRubrique, gr0.classementSousRubrique, 
  gr0.rubrique,gr0.sousRubrique, gr0.libelle, 
   gr4.valeur as 'MMP',
   gr5.valeur as 'INPR',
   gr6.valeur as 'CAPREVAL',
   gr7.valeur as 'CMAV',
   gr8.valeur as 'MUT2M',
   gr9.valeur as 'SAPREM',   
   gr10.valeur as 'AUXIA',
   gr11.valeur as 'QUATREM',
   gr12.valeur as 'AUTRES',
   gr13.valeur as 'ASSURANCE',
   (gr13_amount.valeur/NULLIF(gr13_all.valeur,0)) as ' '
  from PTF_RAPPORT_NIV2 as gr0
  left outer join PTF_RAPPORT_NIV2 as gr4 on gr4.rubrique = gr0.rubrique and gr4.sousRubrique = gr0.sousRubrique 
  and gr4.Groupe= 'MMP' and gr4.cle = @rapportCle and gr4.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr5 on gr5.rubrique = gr0.rubrique and gr5.sousRubrique = gr0.sousRubrique 
  and gr5.Groupe= 'INPR' and gr5.cle = @rapportCle and gr5.date = @date
    left outer join PTF_RAPPORT_NIV2 as gr6 on gr6.rubrique = gr0.rubrique and gr6.sousRubrique = gr0.sousRubrique 
  and gr6.Groupe= 'CAPREVAL' and gr6.cle = @rapportCle and gr6.date = @date  
  left outer join PTF_RAPPORT_NIV2 as gr7 on gr7.rubrique = gr0.rubrique and gr7.sousRubrique = gr0.sousRubrique 
  and gr7.Groupe= 'CMAV' and gr7.cle = @rapportCle and gr7.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr8 on gr8.rubrique = gr0.rubrique and gr8.sousRubrique = gr0.sousRubrique 
  and gr8.Groupe= 'MUT2M' and gr8.cle = @rapportCle and gr8.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr9 on gr9.rubrique = gr0.rubrique and gr9.sousRubrique = gr0.sousRubrique 
  and gr9.Groupe= 'SAPREM' and gr9.cle = @rapportCle and gr9.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr10 on gr10.rubrique = gr0.rubrique and gr10.sousRubrique = gr0.sousRubrique 
  and gr10.Groupe= 'AUXIA' and gr10.cle = @rapportCle and gr10.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr11 on gr11.rubrique = gr0.rubrique and gr11.sousRubrique = gr0.sousRubrique 
  and gr11.Groupe= 'QUATREM' and gr11.cle = @rapportCle and gr11.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr12 on gr12.rubrique = gr0.rubrique and gr12.sousRubrique = gr0.sousRubrique 
  and gr12.Groupe= 'AUTRES' and gr12.cle = @rapportCle and gr12.date = @date
  left outer join PTF_RAPPORT_NIV2 as gr13 on gr13.rubrique = gr0.rubrique and gr13.sousRubrique = gr0.sousRubrique 
   and gr13.Groupe= 'ASSURANCE' and gr13.cle = @rapportCle and gr13.date = @date  

  left outer join PTF_RAPPORT_NIV2 as gr13_amount on gr13_amount.rubrique = gr0.rubrique and gr13_amount.sousRubrique = gr0.sousRubrique 
   and gr13_amount.Groupe= gr13.Groupe and gr13_amount.cle = @cle_Montant and gr13_amount.date = @date         
  left outer join PTF_RAPPORT_NIV2 as gr13_all on gr13_all.rubrique = @rubriqueEncours and gr13_all.sousRubrique = @sousRubriqueEncours 
   and gr13_all.Groupe= gr13.Groupe and gr13_all.cle = @cle_Montant and gr13_all.date = @date      
   
      where gr0.Groupe= 'FGA' and gr0.cle = @rapportCle and gr0.date = @date
  order by gr0.classementRubrique, gr0.classementSousRubrique


select  
case when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)=0 then 1 
		when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000) then 0
		when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100) then 2
                when gr0.classementRubrique/10=CONVERT(INT,gr0.classementRubrique/10) then 3
                else 4
		end as 'style',
case when gr0.classementRubrique=0 then ''
		when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000) then 'graph1'
		else ''
		end as 'graph',
   100*gr0.classementRubrique +10*gr0.classementSousRubrique
   as 'ordre',  
  gr0.classementRubrique, gr0.classementSousRubrique, 
  gr0.rubrique,gr0.sousRubrique, gr0.libelle, 
   gr14.valeur as 'ARCELOR MITTAL France',
   gr15.valeur as 'IDENTITES MUTUELLE',
   gr16.valeur as 'IRCEM MUTUELLE',
   gr17.valeur as 'IRCEM PREVOYANCE',
   gr18.valeur as 'IRCEM RETRAITE',
   gr19.valeur as 'UNMI',
   gr20.valeur as 'TOTAL EXTERNE',
   (gr20_amount.valeur/NULLIF(gr20_all.valeur,0)) as ' '
  from PTF_RAPPORT_NIV2 as gr0
  left outer join PTF_RAPPORT_NIV2 as gr14 on gr14.rubrique = gr0.rubrique and gr14.sousRubrique = gr0.sousRubrique 
  and gr14.Groupe= 'ARCELOR MITTAL France' and gr14.cle = @rapportCle and gr14.date = @date    
  left outer join PTF_RAPPORT_NIV2 as gr15 on gr15.rubrique = gr0.rubrique and gr15.sousRubrique = gr0.sousRubrique 
  and gr15.Groupe= 'IDENTITES MUTUELLE' and gr15.cle = @rapportCle and gr15.date = @date    
  left outer join PTF_RAPPORT_NIV2 as gr16 on gr16.rubrique = gr0.rubrique and gr16.sousRubrique = gr0.sousRubrique 
  and gr16.Groupe= 'IRCEM MUTUELLE' and gr16.cle = @rapportCle and gr16.date = @date    
  left outer join PTF_RAPPORT_NIV2 as gr17 on gr17.rubrique = gr0.rubrique and gr17.sousRubrique = gr0.sousRubrique 
  and gr17.Groupe= 'IRCEM PREVOYANCE' and gr17.cle = @rapportCle and gr17.date = @date    
  left outer join PTF_RAPPORT_NIV2 as gr18 on gr18.rubrique = gr0.rubrique and gr18.sousRubrique = gr0.sousRubrique 
  and gr18.Groupe= 'IRCEM RETRAITE' and gr18.cle = @rapportCle and gr18.date = @date    
  left outer join PTF_RAPPORT_NIV2 as gr19 on gr19.rubrique = gr0.rubrique and gr19.sousRubrique = gr0.sousRubrique 
  and gr19.Groupe= 'UNMI' and gr19.cle = @rapportCle and gr19.date = @date    
  left outer join PTF_RAPPORT_NIV2 as gr20 on gr20.rubrique = gr0.rubrique and gr20.sousRubrique = gr0.sousRubrique 
  and gr20.Groupe= 'EXTERNE' and gr20.cle = @rapportCle and gr20.date = @date    

  left outer join PTF_RAPPORT_NIV2 as gr20_amount on gr20_amount.rubrique = gr0.rubrique and gr20_amount.sousRubrique = gr0.sousRubrique 
   and gr20_amount.Groupe= gr20.Groupe and gr20_amount.cle = @cle_Montant and gr20_amount.date = @date         
  left outer join PTF_RAPPORT_NIV2 as gr20_all on gr20_all.rubrique = @rubriqueEncours and gr20_all.sousRubrique = @sousRubriqueEncours 
   and gr20_all.Groupe= gr20.Groupe and gr20_all.cle = @cle_Montant and gr20_all.date = @date      
   
      where gr0.Groupe= 'FGA' and gr0.cle = @rapportCle and gr0.date = @date
  order by gr0.classementRubrique, gr0.classementSousRubrique


select  
case when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)=0 then 1 
		when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000) then 0
		when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/100) then 2
                when gr0.classementRubrique/10=CONVERT(INT,gr0.classementRubrique/10) then 3
                else 4
		end as 'style',
case when gr0.classementRubrique=0 then ''
		when (100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000=CONVERT(INT,(100*gr0.classementRubrique +10*gr0.classementSousRubrique)/10000) then 'graph1'
		else ''
		end as 'graph',
   100*gr0.classementRubrique +10*gr0.classementSousRubrique
   as 'ordre',  
  gr0.classementRubrique, gr0.classementSousRubrique, 
  gr0.rubrique,gr0.sousRubrique, gr0.libelle, 
  gr0.valeur as 'FGA(tous les groupes)',
  (gr0_amount.valeur/NULLIF(gr0_all.valeur,0)) as ' '
  from PTF_RAPPORT_NIV2 as gr0
  left outer join PTF_RAPPORT_NIV2 as gr0_amount on gr0_amount.rubrique = gr0.rubrique and gr0_amount.sousRubrique = gr0.sousRubrique 
   and gr0_amount.Groupe= gr0.Groupe and gr0_amount.cle = @cle_Montant and gr0_amount.date = @date         
  left outer join PTF_RAPPORT_NIV2 as gr0_all on gr0_all.rubrique = @rubriqueEncours and gr0_all.sousRubrique = @sousRubriqueEncours 
   and gr0_all.Groupe= gr0.Groupe and gr0_all.cle = @cle_Montant and gr0_all.date = @date         
      where gr0.groupe= 'FGA' and gr0.cle = @rapportCle and gr0.date = @date
  order by gr0.classementRubrique, gr0.classementSousRubrique