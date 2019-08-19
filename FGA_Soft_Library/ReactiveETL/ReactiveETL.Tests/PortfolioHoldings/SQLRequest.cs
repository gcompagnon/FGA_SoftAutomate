using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReactiveETL.Tests.PortfolioHoldings
{
	class SQLRequest
	{
		public static Row Mapping(Row row)
		{
			string name = (string)row["name"];
			row["FirstName"] = name.Split()[0];
			row["LastName"] = name.Split()[1];
			return row;
		}

		public static string SelectPortfolio
		{
			get
			{
				return @"SELECT
			g._NOMGROUPE							as Groupe,
			v._dateoperation						as Date,
			v._compte								as Code,
			c._username								as ISIN,
			c._LIBELLECLI							as Label,
			v._assettype							as CompoCode,
			p._isin									as CompoISIN,
			p._libelle1prod							as CompoLabel,
			sum(v._netamountD)						as CompoAmount,
			sum(v._netamountDcc )					as CompoAccruedCoupon,
			sum(v._netentrypriceD  )				as CompoEntryAmount,
			sum(v._netentrypriceDcc )				as CompoEntryAccruedAmount,
			sum(v._unlossD    )						as CompoWinLoss,
			sum(v._unlossDcc)						as CompoWinLossAccruedCoupon,
			rtrim(v._libelletypeproduit) + (case when (( v._libelletypeproduit ='CALL' OR  v._libelletypeproduit ='PUT') AND left(ss._LIBELLESOUSSECTEUR,3)='EMP') then ' TAUX' 		else '' end) as CompoType, 
			v._deviseS								as CompoCurrency,
			s._LIBELLESECTEUR						as CompoSector,
			ss._LIBELLESOUSSECTEUR					as CompoSubSector,
			pi._LIBELLEPAYS							as CompoGrpIssuerCountry,
			e._NOMEMETTEUR							as CompoIssuer,
--            r._signature							as CompoRating,
			GR._libellegrper						as CompoIssuerGrp,
			case when ((( v._libelletypeproduit ='CALL' OR  v._libelletypeproduit ='PUT') AND left(ss._LIBELLESOUSSECTEUR,3)<>'EMP') OR 		(left(v._libelletypeproduit,12)='Certificat A') OR (left(v._libelletypeproduit,9)='FUTURES A' ) ) then NULL else p._echeance end as CompoMaturityDate,
			h._duration								as CompoDuration,
			case when h._rendement=0 then h._sensibilite else h._duration/(1+(h._rendement)/100) end as CompoModifiedDuration,
			h._coursclose							as CompoClosePrice,
			case when p._methodecot=3 then	sum( v._quantite*p._nominal) else sum( v._quantite) end as CompoQuantiry,
			sum(p._nominal) as CompoNominal,
			p._coupon As CompoCoupon,
			case when h._rendement<>0 or h._sensibilite=0 then h._rendement else (h._duration/h._sensibilite -1 ) *100 end as CompoYTM
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
		 v._dateoperation='***' and
		 g._codegroupe in (%%%) and
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
			h._rendement";
			}
		}
	}
}
