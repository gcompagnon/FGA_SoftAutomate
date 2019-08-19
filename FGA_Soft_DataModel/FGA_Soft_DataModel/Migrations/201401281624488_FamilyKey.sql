ALTER TABLE [ref_holding].[VALUATION] ADD [IndexDivisor] [float]
ALTER TABLE [ref_holding].[VALUATION] ADD [IndexNumberOfSecurities] [int]
ALTER TABLE [ref_security].[EQUITY] ADD [FamilyKey] [varbinary](max)
ALTER TABLE [ref_holding].[INDEX] ADD [FamilyKey] [varbinary](max)
ALTER TABLE [ref_security].[EQUITY] ALTER COLUMN [FamilyKey] [nvarchar](max)
ALTER TABLE [ref_holding].[INDEX] ALTER COLUMN [FamilyKey] [nvarchar](max)


ALTER TABLE [ref_security].[EQUITY] ADD [FamilyKey] [nvarchar](max)
ALTER TABLE [ref_holding].[INDEX] ADD [FamilyKey] [nvarchar](max)