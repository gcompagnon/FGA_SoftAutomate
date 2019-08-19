-- Fichier en encodage UTF-8 (ANSI ou ASCII fonctionne si aucun caractères accentués)

-- REQUETE DE SYNCHRONISATION entre OMEGA et la base FGA Soft
--  exploite les inventaires validés suivant les différents groupes
--  Récupère les mandats et opcvm pour une date donnée
--  Le traitement est différent suivant: 
--        Les futures
--        
--------------------------------------------------------------------------------------------------------------------------------

--- A voir/TODO: la table fcp.OPERAT qui contient les opérations du jour liées au carnet d ordre fcp.CARNETOR par _numeroordre
---  le gérant sauvegarde une opé dans le carnet d ordre(statut sauvegardé MO et transféré MO) _TRANSFERERMO =1 
---  le MO valide l'opé pour envoyer un swift , et mettre dans la prochaine valo  
---  le MO met dans l'état ValiderBO pour attendre de fixer une VL (ordre sur OPCVM)
--         ou dans l'état retour conservateur si le prix est déjà connu
-- select top 100 c._CODEPRODUI,c._compte,c._codeoperation,c._codeoperateur,o._NOMINAL,o._QUANTITE,o._DATEVL,o._TRANSFERERMO, o._VALIDMO,o._EXPORTED, o._CONFIRM, o._VALORISEE, o._RECONCILIATED,
--  c.*,o.* from fcp.carnetor as c
--  left outer join fcp.OPERAT as o on o._NUMEROORDRE = c._NUMEROORDRE
--  order by c._datedebut desc
  

--- A voir/TODO: mettre la création des lignes Cash contrebalancant les positions Futures dans la proc stock GetCashFgaX.sql


--------------------------------------------------------------------------------------------------------------------------------
--- Description de la table fcp.VALOPRDT  (table des fonds valorisés à end-of-day):
---                     'S' comme Stock  : _netAmountS , _netEntryPriceS et  _unlossS , le cours en devise Produit (alimentation SIX telekurs)
---                     'D' comme Devise : _netAmountD , _netEntryPriceD et  _unlossD , le cours en devise Fond (euro) , cette ligne est reconciliée avec les inventaires validés (où le forex provient de BP2S)
---                     'M' comme Market : _netAmountM , _netEntryPriceM et  _unlossM , le cours en devise Fond (euro) est calculé par Omega avec le forex stocké (cours à l'entree de la ligne en ptf)
---                     'I' comme Indicatif : non utilisé 

---         les données d analyse (duration , YTM/rendement, sensibilite/ModifiedDuration ..)  sont avec les prix dans com.prixhist 
---         sur com.produit  methodedecot est un code (voir com.METHODECOT_CORR ) (null pour les actions)
--                           methodedecot = 1 (en piece), 3 en % Pdecimal en 32ième
---                          _codedutype lié à com.typeprod
---                           _codeinterets à com.frequenceint

--------------------------------------------------------------------------------------------------------------------------------
USE OMEGA

declare @dateExtraction datetime
set @dateExtraction = '20/06/2012'
--set @dateExtraction = '***'

declare @option varchar(10)
--set @option = 'SIMPLE'
set @option = 'RISK'

declare @group table
(
  Groupe varchar(25)
)
insert into @group(Groupe)  Values ('M')
--insert into @group(Groupe)  Values ('IG')
--insert into @group(Groupe)  Values ('IO')
--insert into @group(Groupe)  Values ('UM')
--insert into @group(Groupe)  Values ('IN')
--insert into @group(Groupe)  Values ('CA')
--insert into @group(Groupe)  Values ('CT')
--insert into @group(Groupe)  Values ('MM')
--insert into @group(Groupe)  Values ('SA')
--insert into @group(Groupe)  Values ('SD')
--insert into @group(Groupe)  Values ('QX')
--insert into @group(Groupe)  Values ('GA')
--insert into @group(Groupe)  Values ('OP')
--insert into @group(Groupe)  Values ('A6')
--insert into @group(Groupe)  Values ('A4')
--insert into @group(Groupe)  Values ('A5')
--insert into @group(Groupe)  Values ('UN')
--insert into @group(Groupe)  Values ('SM')
--insert into @group(Groupe)  Values ('MN')

--------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------ETAPE 1 : Inventaire

-- Partie 1: tous actifs autres que les Futures ( and v._libelletypeproduit like 'FUTURES %' )
--           le type produit est enrichi avec TAUX pour les options sur emprunts d etat(govies)
--           pas de date de maturité pour les options, certificats Actions et Futures Actions

SELECT
            g._NOMGROUPE							as Groupe,
            v._dateoperation						as Dateinventaire,
            v._compte								as Compte,
            c._username								as ISIN_Ptf,
            c._LIBELLECLI							as Libelle_Ptf,
            v._assettype							as code_Titre,
            p._isin									as isin_titre,
            p._libelle1prod							as Libelle_Titre,
            sum(v._netamountD)						as Valeur_Boursiere,
            sum(v._netamountDcc )					as Coupon_Couru,
            sum(v._netentrypriceD  )				as Valeur_Comptable,
            sum(v._netentrypriceDcc )				as Coupon_Couru_Comptable,
            sum(v._unlossD    )						as PMV,
            sum(v._unlossDcc)						as PMV_CC,
            rtrim(v._libelletypeproduit) + (case when (( v._libelletypeproduit ='CALL' OR  v._libelletypeproduit ='PUT') AND left(ss._LIBELLESOUSSECTEUR,3)='EMP') then ' TAUX' 		else '' end) as Type_Produit, 
            v._deviseS								as Devise_Titre,
            s._LIBELLESECTEUR						as Secteur,
            ss._LIBELLESOUSSECTEUR					as Sous_Secteur,
            pi._LIBELLEPAYS							as Pays,
            e._NOMEMETTEUR							as Emetteur,
--            r._signature							as Rating,
            GR._libellegrper						as Grp_Emetteur,
            case when ((( v._libelletypeproduit ='CALL' OR  v._libelletypeproduit ='PUT') AND left(ss._LIBELLESOUSSECTEUR,3)<>'EMP') OR 		(left(v._libelletypeproduit,12)='Certificat A') OR (left(v._libelletypeproduit,9)='FUTURES A' ) ) then NULL else p._echeance end as maturite,
            h._duration								as duration,
			case when h._rendement=0 then h._sensibilite else h._duration/(1+(h._rendement)/100) end as sensibilite,
            h._coursclose							as coursclose,
            case when p._methodecot=3 then	sum( v._quantite*p._nominal) else sum( v._quantite) end as quantite,
            sum(p._nominal) as nominal,
			p._coupon As coupon,
			case when h._rendement<>0 or h._sensibilite=0 then h._rendement else (h._duration/h._sensibilite -1 ) *100 end as rendement
into #INVENTAIRE			
FROM
          fcp.valoprdt  v
left outer join           com.produit p                 on p._codeprodui=v._assettype
left outer join			  com.prdtclasspys ps			on p._codeprodui=ps._codeprodui and ps._classification=0
left outer join           com.soussect ss               on ss._CODESOUSSECTEUR=ps._codesoussecteur
left outer join			  com.ssectclass ss_s			on ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR and ss_s._classification=0
left outer join           com.secteurs s                on s._CODESECTEUR=ss_s._codesecteur
--left outer join           com.rating r					on r._codeprodui=p._codeprodui and r._codeagence='INT' and r._date=(select max(_date) from com.rating r where r._date<=v._dateoperation and r._codeprodui=p._codeprodui)
left outer join           com.prixhist  h               on v._datecours=h._date and p._codeprodui=h._codeprodui
left outer join           com.emetteur e                on e._emetteur=p._emetteur
left outer join           com.grpemetteursratios GR     on  e._grper=GR._codegrper
left outer join           com.pays pi                   on pi._CODEPAYS= gr._codepays
left outer join           fcp.cpartfcp c                on  c._compte=v._compte
left outer join           Fcp.CONSGRPE cg               on cg._compte=v._compte
left outer join           fcp.grpedeft g                on g._codegroupe=cg._codegroupe
WHERE          
         v._dateoperation=@dateExtraction and
	     g._codegroupe in (select Groupe from @Group) and
	     v._libelletypeproduit not like 'FUTURES %'
group by
			g._NOMGROUPE,
            v._dateoperation ,
            v._compte,
            c._username,
            c._LIBELLECLI,
            v._assettype,
            p._isin,  
            p._libelle1prod,
            v._libelletypeproduit, 
            v._deviseS, 
            s._LIBELLESECTEUR, 
            ss._LIBELLESOUSSECTEUR,
            pi._LIBELLEPAYS, 
            e._NOMEMETTEUR,
--            r._signature,
			GR._libellegrper,
			p._echeance,
			h._duration,
			h._sensibilite,
			h._rendement,
			h._coursclose,
			p._methodecot,
			p._coupon,
			h._rendement
--ORDER BY g._NOMGROUPE ,  v._compte,  p._isin

-- Partie 2.1: les Futures : doubler les lignes: 
--               1 ligne de type future
--               2 ligne de type cash pour avoir somme(Valeur Boursiere) = PMV et somme(Valeur Comptable) = 0
insert into #INVENTAIRE
SELECT
            g._NOMGROUPE							as Groupe,
            v._dateoperation						as Dateinventaire,
            v._compte								as Compte,
            c._username								as ISIN_Ptf,
            c._LIBELLECLI							as Libelle_Ptf,
            v._assettype							as code_Titre,
            p._isin 								as isin_titre,  
            p._libelle1prod							as Libelle_Titre,
            sum( (case when (v._BS = 'V') then -1 else 1 end) *p._multiple*v._marketpriceD*v._quantite ) as Valeur_Boursiere,
            sum(v._netamountDcc )					as Coupon_Couru,
			sum( (case when (v._BS = 'V') then -1 else 1 end) *p._multiple*v._entrypriceD*v._quantite ) as Valeur_Comptable,
            sum(v._netentrypriceDcc )				as Coupon_Couru_Comptable,  
            sum(v._unlossD)						as PMV,  
            sum(v._unlossDcc)						as PMV_CC,
            rtrim(v._libelletypeproduit) as Type_Produit, 
            v._deviseS								as Devise_Titre, 
            s._LIBELLESECTEUR						as Secteur, 
            ss._LIBELLESOUSSECTEUR					as Sous_Secteur,
            pi._LIBELLEPAYS							as Pays, 
            e._NOMEMETTEUR							as Emetteur,
--            r._signature							as Rating,
            GR._libellegrper						as Grp_Emetteur,   
            case when (left(v._libelletypeproduit,9)='FUTURES A' ) then NULL else p._echeance end as maturite,
            h._duration								as duration,
			case when h._rendement=0 then h._sensibilite else h._duration/(1+(h._rendement)/100) end as sensibilite,
            h._coursclose							as coursclose,
            case when p._methodecot=3 then	sum( v._quantite*p._nominal) else sum( v._quantite) end as quantite,
            sum(p._nominal) as nominal,
			p._coupon As coupon,
			h._rendement As rendement
FROM
          fcp.valoprdt  v
left outer join           com.produit p                 on p._codeprodui=v._assettype
left outer join			  com.prdtclasspys ps			on p._codeprodui=ps._codeprodui and ps._classification=0
left outer join           com.soussect ss               on ss._CODESOUSSECTEUR=ps._codesoussecteur
left outer join			  com.ssectclass ss_s			on ss._CODESOUSSECTEUR=ss_s._CODESOUSSECTEUR and ss_s._classification=0
left outer join           com.secteurs s                on s._CODESECTEUR=ss_s._codesecteur
--left outer join           com.rating r					on r._codeprodui=p._codeprodui and r._codeagence='INT' and r._date=(select max(_date) from com.rating r where r._date<=v._dateoperation and r._codeprodui=p._codeprodui)
left outer join           com.prixhist  h               on v._datecours=h._date and p._codeprodui=h._codeprodui
left outer join           com.emetteur e                on e._emetteur=p._emetteur
left outer join           com.grpemetteursratios GR     on  e._grper=GR._codegrper
left outer join           com.pays pi                   on pi._CODEPAYS= gr._codepays
left outer join           fcp.cpartfcp c                on  c._compte=v._compte
left outer join           Fcp.CONSGRPE cg               on cg._compte=v._compte
left outer join           fcp.grpedeft g                on g._codegroupe=cg._codegroupe
WHERE          
         v._dateoperation=@dateExtraction and
	     g._codegroupe in (select Groupe from @Group) and
	     v._libelletypeproduit like 'FUTURES %'	     
group by
			g._NOMGROUPE,
            v._dateoperation ,
            v._compte,
            c._username,
            c._LIBELLECLI,
            v._assettype,
            p._isin,  
            p._libelle1prod,
            v._libelletypeproduit, 
            v._deviseS, 
            s._LIBELLESECTEUR, 
            ss._LIBELLESOUSSECTEUR,
            pi._LIBELLEPAYS, 
            e._NOMEMETTEUR,
--            r._signature,
			GR._libellegrper,
			p._echeance,
			h._duration,
			h._sensibilite,
			h._rendement,
			h._coursclose,
			p._methodecot,
			p._coupon,
			h._rendement
--ORDER BY g._NOMGROUPE ,  v._compte,  p._isin

---- Partie 2.2: les Futures : doubler les lignes: 
----               1 ligne de type future
----               2 ligne de type cash pour avoir somme(Valeur Boursiere) = PMV et somme(Valeur Comptable) = 0

insert into #INVENTAIRE
SELECT
            g._NOMGROUPE							as Groupe,
            v._dateoperation						as Dateinventaire,
            v._compte								as Compte,
            c._username								as ISIN_Ptf,
            c._LIBELLECLI							as Libelle_Ptf,
            rtrim(v._assettype) + '_cash'					as code_Titre,
            rtrim(p._isin) + '_cash'  								as isin_titre,  
            'Liquidité(Future)'						as Libelle_Titre,
            sum( (case when (v._BS = 'A') then -1 else 1 end) *p._multiple*v._entrypriceD*v._quantite ) as Valeur_Boursiere,
            0										as Coupon_Couru,
			sum( (case when (v._BS = 'A') then -1 else 1 end) *p._multiple*v._entrypriceD*v._quantite ) as Valeur_Comptable,
            0										as Coupon_Couru_Comptable,  
            0										as PMV,  
            0										as PMV_CC,
            'Cash'									as Type_Produit, 
            'EUR'									as Devise_Titre, 
            'Liquidité'								as Secteur, 
            'Liquidité'								as Sous_Secteur,
            NULL									as Pays,  -- pas d expo sur le Pays France, mais sur l'EURO . voir pour un faux pays
            NULL									as Emetteur,
--            NULL									as Rating,
            NULL									as Grp_Emetteur,   
            NULL									as maturite,
            0										as duration,
			0										as sensibilite,
            NULL									as coursclose,
            0										as quantite,
            0										as nominal,
			0										as coupon,
			0										as rendement
FROM
          fcp.valoprdt  v
left outer join           com.produit p                 on p._codeprodui=v._assettype
left outer join           fcp.cpartfcp c                on  c._compte=v._compte
left outer join           Fcp.CONSGRPE cg               on cg._compte=v._compte
left outer join           fcp.grpedeft g                on g._codegroupe=cg._codegroupe
WHERE          
         v._dateoperation=@dateExtraction and
	     g._codegroupe in (select Groupe from @Group) and
	     v._libelletypeproduit like 'FUTURES %'	     
group by
			g._NOMGROUPE,
            v._dateoperation ,
            v._compte,
            c._username,
            c._LIBELLECLI,
            v._assettype,
            p._isin
            
--ORDER BY g._NOMGROUPE ,  v._compte,  p._isin

--------------------------------------------------------------------------------------------------------------------------------
------------------------------ ETAPE RATING
select 
    max(r._date) as C33_DateRating,
    r._codeprodui as C05_code_Titre
into #RATINGDATE
from #INVENTAIRE as i
left outer join com.rating as r on r._codeprodui=i.code_Titre and r._codeagence='INT' and r._date<=i.Dateinventaire
group by  r._codeprodui

select 
    r._signature as C32_Rating,
    r._date as C33_DateRating,
    r._codeprodui as C05_code_Titre
into #RATING
from #RATINGDATE as i
left outer join com.rating as r on r._codeprodui=i.C05_code_Titre and r._codeagence='INT' and r._date=i.C33_DateRating
------------------------------ ETAPE OBLIG CALLABLE ou PERPETUAL : prendre la 1ere date de call à la place de l echeance/maturite

select code_Titre into #CALLABLE_PERPETUEL
from #INVENTAIRE as i
left outer join com.produit p on p._codeprodui=i.code_Titre
where p._callable = 1 or p._codedutype = 'OPERP'

-- pour les callable, prendre la prochaine date  call
-- pour les perp, prendre la date de prochain tombee de coupon
SELECT _codeprodui as code_Titre , min (_DATE ) as maturite
into #CALLDATE
  FROM [OMEGA].[Com].[FLUX] where _codeprodui in (select code_Titre from #CALLABLE_PERPETUEL) and _Date > getDate()
group by   _codeprodui

-----------------------------SORTIE : option simple pour l integration dans la base FGA
if @option = 'SIMPLE'
begin

	select i.Groupe,i.Dateinventaire,i.Compte,i.ISIN_Ptf,i.Libelle_Ptf,i.code_Titre,i.isin_titre,i.Libelle_Titre,i.Valeur_Boursiere,i.Coupon_Couru,i.Valeur_Comptable,i.Coupon_Couru_Comptable,i.PMV,i.PMV_CC,i.Type_Produit,i.Devise_Titre,i.Secteur,i.Sous_Secteur,i.Pays,i.Emetteur,i.Grp_Emetteur,i.duration,i.sensibilite,i.coursclose,i.quantite,i.nominal,i.coupon,i.rendement,
	case 
	when c.maturite is null then i.maturite
	else c.maturite end as maturite,
	 r.C32_Rating as rating
	from #INVENTAIRE as i
	left outer join #RATING as r on r.C05_code_Titre = i.code_Titre
	left outer join #CALLDATE as c on c.code_Titre = i.code_Titre
end
-----------------------------SORTIE : option aggregation pour sortie base mandat en mode Risk
else if @option='RISK'
begin

   --
   create table #ref_transco ( transco varchar(10) not null, input varchar(60), output varchar(60)  )
   -- attribution à une classe d actifs
   insert into #ref_transco values ('ASSETCLASS', 'Actions Allemagne', 'ACTION')
insert into #ref_transco values ('ASSETCLASS', 'Actions Autriche', 'ACTION')
insert into #ref_transco values ('ASSETCLASS', 'Actions Finlande', 'ACTION')
insert into #ref_transco values ('ASSETCLASS', 'Actions France', 'ACTION')
insert into #ref_transco values ('ASSETCLASS', 'Actions Grèce', 'ACTION')
insert into #ref_transco values ('ASSETCLASS', 'Actions Italie', 'ACTION')
insert into #ref_transco values ('ASSETCLASS', 'Actions Luxembourg', 'ACTION')
insert into #ref_transco values ('ASSETCLASS', 'Actions Pays-bas', 'ACTION')
insert into #ref_transco values ('ASSETCLASS', 'Droits d''attribution', 'DA')
insert into #ref_transco values ('ASSETCLASS', 'Fonds Commun de Créance', 'FCC')
insert into #ref_transco values ('ASSETCLASS', 'Fonds Commun de Placement', 'FCP')
insert into #ref_transco values ('ASSETCLASS', 'Obligations Convertibles', 'OBLIGATION')
insert into #ref_transco values ('ASSETCLASS', 'Obligations Indexées sur l''Inflation', 'OBLIGATION')
insert into #ref_transco values ('ASSETCLASS', 'Obligations structurées', 'OBLIGATION')
insert into #ref_transco values ('ASSETCLASS', 'Obligations Taux Fixe', 'OBLIGATION')
insert into #ref_transco values ('ASSETCLASS', 'Obligations Taux Fixe avec Call', 'OBLIGATION')
insert into #ref_transco values ('ASSETCLASS', 'Obligations Taux Variable', 'OBLIGATION')
insert into #ref_transco values ('ASSETCLASS', 'Obligations Taux Variable avec Call', 'OBLIGATION')
insert into #ref_transco values ('ASSETCLASS', 'Obligations tx fixe perpetuelles', 'OBLIGATION')
insert into #ref_transco values ('ASSETCLASS', 'Sicav', 'SICAV')
insert into #ref_transco values ('ASSETCLASS', 'Sicav USD', 'SICAV')
insert into #ref_transco values ('ASSETCLASS', 'DEFAULT', 'NA')

insert into #ref_transco values ('FINCLASS', 'CORPORATES FINANCIERES ASSURANCES', 'FIN')
insert into #ref_transco values ('FINCLASS', 'CORPORATES FINANCIERES BANQUES', 'FIN')
insert into #ref_transco values ('FINCLASS', 'CORPORATES FINANCIERES SERVICES FINANCIERS', 'FIN')
insert into #ref_transco values ('FINCLASS', 'SOCIETES FINANCIERES', 'FIN')
insert into #ref_transco values ('FINCLASS', 'DEFAULT', 'NONFIN')

insert into #ref_transco values ('ZONEGEO', 'FRANCE', 'Zone 1')
insert into #ref_transco values ('ZONEGEO', 'ALLEMAGNE', 'Zone 1')
insert into #ref_transco values ('ZONEGEO', 'PAYS-BAS', 'Zone 1')
insert into #ref_transco values ('ZONEGEO', 'FINLANDE', 'Zone 1')
insert into #ref_transco values ('ZONEGEO', 'AUTRICHE', 'Zone 1')
insert into #ref_transco values ('ZONEGEO', 'SUPRANATIONAL', 'Zone 1')
insert into #ref_transco values ('ZONEGEO', 'PORTUGAL', 'Zone 2')
insert into #ref_transco values ('ZONEGEO', 'ITALIE', 'Zone 2')
insert into #ref_transco values ('ZONEGEO', 'IRLANDE', 'Zone 2')
insert into #ref_transco values ('ZONEGEO', 'GRÈCE', 'Zone 2')
insert into #ref_transco values ('ZONEGEO', 'ESPAGNE', 'Zone 2')
insert into #ref_transco values ('ZONEGEO', 'BELGIQUE', 'Extension Zone 1')
insert into #ref_transco values ('ZONEGEO', 'LUXEMBOURG', 'Extension Zone 1')
insert into #ref_transco values ('ZONEGEO', 'DANEMARK', 'Extension Zone 1')
insert into #ref_transco values ('ZONEGEO', 'ROYAUME-UNI', 'Europe-ex euro')
insert into #ref_transco values ('ZONEGEO', 'SUÈDE', 'Europe-ex euro')
insert into #ref_transco values ('ZONEGEO', 'NORVÈGE', 'Europe-ex euro')
insert into #ref_transco values ('ZONEGEO', 'SUISSE', 'Europe-ex euro')
insert into #ref_transco values ('ZONEGEO', 'RÉPUBLIQUE TCHÈQUE', 'Europe-ex euro')
insert into #ref_transco values ('ZONEGEO', 'HONGRIE', 'Europe-ex euro')
insert into #ref_transco values ('ZONEGEO', 'JERSEY', 'Europe-ex euro')
insert into #ref_transco values ('ZONEGEO', 'ISLANDE', 'Europe-ex euro')
insert into #ref_transco values ('ZONEGEO', 'ETATS-UNIS', 'Amérique du Nord + Australie')
insert into #ref_transco values ('ZONEGEO', 'AUSTRALIE', 'Amérique du Nord + Australie')
insert into #ref_transco values ('ZONEGEO', 'CANADA', 'Amérique du Nord + Australie')
insert into #ref_transco values ('ZONEGEO', 'DEFAULT', 'Reste')




select i.Groupe,i.Dateinventaire,i.Compte,i.ISIN_Ptf,i.Libelle_Ptf,i.code_Titre,i.isin_titre,i.Libelle_Titre,i.Valeur_Boursiere,i.Coupon_Couru,i.Valeur_Comptable,i.Coupon_Couru_Comptable,i.PMV,i.PMV_CC,i.Type_Produit,i.Devise_Titre,i.Secteur,i.Sous_Secteur,i.Pays,i.Emetteur,i.Grp_Emetteur,i.duration,i.sensibilite,i.coursclose,i.quantite,i.nominal,i.coupon,i.rendement,
	i.maturite as maturite, c.maturite as nextcalldate,
	 r.C32_Rating as rating,
	 ISNULL(t1.output,t1d.output) as FINCLASS,
	 ISNULL(t2.output,t2d.output) as ASSETCLASS,
	 ISNULL(t3.output,t3d.output) as ZONE_GEO
	from #INVENTAIRE as i
	left outer join #RATING as r on r.C05_code_Titre = i.code_Titre
	left outer join #CALLDATE as c on c.code_Titre = i.code_Titre
	left outer join #ref_transco as t1 on t1.transco = 'FINCLASS' and t1.input =  i.Secteur 
	left outer join #ref_transco as t1d on t1d.transco = 'FINCLASS' and t1d.input ='DEFAULT'
	left outer join #ref_transco as t2 on t2.transco = 'ASSETCLASS' and t2.input = i.Type_produit
	left outer join #ref_transco as t2d on t2d.transco = 'ASSETCLASS' and t2d.input ='DEFAULT'
	left outer join #ref_transco as t3 on t3.transco = 'ZONEGEO' and t3.input = UPPER(i.Pays)
	left outer join #ref_transco as t3d on t3d.transco = 'ZONEGEO' and t3d.input = 'DEFAULT'

end   
   
drop table  #ref_transco 
drop table #INVENTAIRE
drop table #CALLDATE
drop table #RATINGDATE
drop table #RATING
drop table #CALLABLE_PERPETUEL
