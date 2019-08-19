    select  
            'ALTER TABLE [' + so.Table_Schema + '].[' + so.Table_Name + '] DROP CONSTRAINT ' 
            + so.constraint_name  
            from INFORMATION_SCHEMA.TABLE_CONSTRAINTS so
            where 
            so.TABLE_SCHEMA in ('ref_common','ref_holding','ref_issuer','ref_rating','ref_security')

--use FGA_DATAMODEL
use E2DBFGA01
declare @cmd varchar(4000)
            declare cmds cursor for 
            Select
                'drop table [' + Table_Schema + '].[' + Table_Name + ']'
            From
                INFORMATION_SCHEMA.TABLES
			order by Table_Schema
			
            open cmds
            while 1=1
            
            begin
                fetch cmds into @cmd
                if @@fetch_status != 0 break
                print @cmd
--                exec(@cmd)
            end
            close cmds
            deallocate cmds
            
            Select *
            FROM INFORMATION_SCHEMA.TABLES T 
            Where T.TABLE_NAME = 'EdmMetaData'
            
     
            
drop table [integration_indice].[DEBT]
drop table [integration_indice].[SECURITY_PRICING]
drop table [integration_indice].[ASSET_CLASSIFICATION]
drop table [integration_indice].[INDEX_ASSET]
drop table [integration_indice].[INDEX_DEF]
drop table [integration_indice].[SECURITY]

ALTER TABLE [ref_security].[ASSET] DROP CONSTRAINT PK__ASSET__1DD13137
ALTER TABLE [ref_holding].[COMPONENT] DROP CONSTRAINT PK__COMPONENT__275A9B71
ALTER TABLE [ref_holding].[PORTFOLIO] DROP CONSTRAINT PK__PORTFOLIO__30E405AB
ALTER TABLE [ref_rating].[RATING] DROP CONSTRAINT PK__RATING__32CC4E1D

ALTER TABLE [ref_common].[IDENTIFICATION] DROP CONSTRAINT PK__IDENTIFICATION__369CDF01
ALTER TABLE [ref_holding].[COMPONENT] DROP CONSTRAINT Component_Parent
ALTER TABLE [ref_holding].[PORTFOLIO] DROP CONSTRAINT Portfolio_Identification1
ALTER TABLE [ref_holding].[PORTFOLIO] DROP CONSTRAINT Portfolio_TypeConstraint_From_Component_To_PORTFOLIO
ALTER TABLE [ref_rating].[RATING] DROP CONSTRAINT Rating_RatedSecurity
ALTER TABLE [ref_security].[ASSET] DROP CONSTRAINT Security_Identification
ALTER TABLE [ref_security].[ASSET] DROP CONSTRAINT Security_Rating

drop table [ref_common].[IDENTIFICATION]
drop table [ref_holding].[PORTFOLIO]
drop table [ref_holding].[INDEX]
drop table [ref_holding].[COMPONENT]
drop table [ref_holding].[ASSET_HOLDING]
drop table [ref_holding].[VALUATION]
drop table [ref_issuer].[ROLE]
drop table [ref_rating].[RATING]
drop table [ref_security].[PRICE]
drop table [ref_security].[ASSET_CLASSIFICATION]
drop table [ref_security].[ASSET_PORTFOLIO]
drop table [ref_security].[SECURITIES_ISSUANCE]
drop table [ref_security].[DEBT]
drop table [ref_security].[EQUITY]
drop table [ref_security].[FUND]
drop table [ref_security].[ASSET]            