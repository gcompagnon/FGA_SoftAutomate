
CREATE TABLE [ref_security].[FX_RATE](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[UnitCurrency] [nchar](4) NULL,
	[QuotedCurrency] [nchar](4) NULL,
	[FX] [float] NULL,
 CONSTRAINT [PK_ref_security.FX_RATE] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[Date] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


