using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using FGABusinessComponent.BusinessComponent.Util;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure;
using FGABusinessComponent.BusinessComponent.Common;
using FGABusinessComponent.BusinessComponent;
using FGABusinessComponent.BusinessComponent.Util.XML;
using FGABusinessComponent.BusinessComponent.Core;
using FGABusinessComponent.BusinessComponent.Core.Composite;
using FGABusinessComponent.BusinessComponent.Security;
using FGABusinessComponent.BusinessComponent.Holding;
using FGABusinessComponent.BusinessComponent.Holding.IndexComp;
using FGABusinessComponent.BusinessComponent.Holding.PortfolioComp;
using FGABusinessComponent.BusinessComponent.Security.Roles;
using FGABusinessComponent.BusinessComponent.Security.Pricing;
using System.Data.Entity.Validation;
using FGABusinessComponent.BusinessComponent.Security.Fx;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;



namespace FGABusinessComponent.BusinessComponent
{
#if DEBUG
    /// <summary>
    /// Strategie d'intialisation de la base en mode Debug
    /// un fichier FGACOntext.xml peut contenir des données d initialisation
    /// </summary>
    public class FGAContextInitializer :
              DropCreateDatabaseAlways<FGAContext>
//              DropCreateDatabaseIfModelChanges<FGAContext>
//                DontDropDbJustCreateTablesIfModelChanged<FGAContext>
    {
        private readonly string _xmlSeedFilename;

        /// <summary>
        /// instructions à passer à l initialisation du model de données
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(FGAContext context)
        {
            // creation des contraintes pour la relation one-to-one 
// RatingId peut etre null: pas de constraint possible            context.Database.ExecuteSqlCommand("ALTER TABLE " + Asset.SHEMA_NAME + "." + Asset.TABLE_NAME + " ADD CONSTRAINT UC_Asset_Rating UNIQUE (RatingId)");
            context.Database.ExecuteSqlCommand("ALTER TABLE " + Rating.SHEMA_NAME + "." + Rating.TABLE_NAME + " ADD CONSTRAINT UC_Rating_Asset UNIQUE (ISINId,AssetId)");

            /// Initialisation à partir d un fichier XML 
            XmlSeeder seeder = new XmlSeeder();
            seeder.Seed(context, _xmlSeedFilename);

            base.Seed(context);
        }
    }
#endif

    /// <summary>
    /// le gestionnaire entity framework 
    /// Chaque composant doit avoir des attributs non nuls (nécessaire par les stratégies d initialisation)
    /// </summary>
    public class FGAContext : DbContext
    {
        /// <summary>
        /// Le modele utilise pour creer le MDD de la base de donnée
        /// </summary>
        private static DbCompiledModel compiledModel;
        /// <summary>
        /// constructeur static
        /// </summary>
        static FGAContext()
        {
            // configuration pour que la persistance soit compatible avec le type de base fourni.
            DbModelBuilder modelBuilder = new DbModelBuilder();        
            compiledModel = modelBuilder.Build(new DbProviderInfo("System.Data.SqlClient", "2005")).Compile();
        }

        /// <summary>
        /// Constructeur utilisé par Entity framework Code First Migrations
        /// </summary>
        public FGAContext()
            : base("name=DEV")
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="env">environnement : par exemple "PROD" ou PREPROD comme defini dans App.config</param>
        /// <param name="model"></param>
        /// <param name="performanceOverValidationFlag"></param>
        public FGAContext(string env ="PREPROD",DbCompiledModel model = null, Boolean performanceOverValidationFlag = false)
#if DEBUG
            : base("name=DEV")
        {
            Database.SetInitializer<FGAContext>(new FGAContextInitializer());
#else 
            : base("name="+env)//,model = compiledModel)
        {
            // pas de création du datamodel/modele de données : la base est deja prete            
            Database.SetInitializer<FGAContext> (null);            
#endif

            if (performanceOverValidationFlag)
            {
                // For performance reasons: desactivate features :
                this.Configuration.AutoDetectChangesEnabled = false;
                this.Configuration.ValidateOnSaveEnabled = false;               
            }
            this.Configuration.LazyLoadingEnabled = true; 
        }

        /// <summary>
        /// Les business components: indice, portefeuille, actif en position
        /// </summary>

        public DbSet<Asset> Assets { get; set; }
        public DbSet<FGABusinessComponent.BusinessComponent.Security.Security> Securities { get; set; }


        public DbSet<Debt> Debts { get; set; }
        public DbSet<Equity> Equities { get; set; }
        public DbSet<InvestmentFund> InvestmentFunds { get; set; }

        public DbSet<Index> Indexes { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<AssetPortfolioAssociation> AssetPortfolioAssociations { get; set; }

        public DbSet<SecuritiesIdentification> SecuritiesIds { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<AssetHolding> AssetHoldings { get; set; }
        public DbSet<SecuritiesPricing> SecuritiesPricings { get; set; }
        public DbSet<Valuation> Valuations { get; set; }
        public DbSet<AssetClassification> AssetClassifications { get; set; }

        public DbSet<IssuerRole> IssuerRoles { get; set; }

        public DbSet<CurrencyExchange> CurrencyExchanges { get; set; }
        /// FIN de la définition des Business Components

        /// <summary>
        /// AJout des profils de configuration au format FLUENT API (complémentaire aux config par Attribut) 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            //modelBuilder.Configurations.Add(new IndexBasketDataModelConfiguration());
            //modelBuilder.Configurations.Add(new IndexRateDataModelConfiguration());
            // partie Holding
            modelBuilder.Configurations.Add(new ComponentModelConfiguration());
            modelBuilder.Configurations.Add(new CompositeModelConfiguration());

            modelBuilder.Configurations.Add(new IndexModelConfiguration());

            modelBuilder.Configurations.Add(new PortfolioModelConfiguration());
            modelBuilder.Configurations.Add(new AssetPortfolioAssociationModelConfiguration());

            modelBuilder.Configurations.Add(new SecuritiesIdentificationModelConfiguration());
            modelBuilder.Configurations.Add(new AssetHoldingModelConfiguration());

            // Partie Security
            modelBuilder.Configurations.Add(new AssetModelConfiguration());
            modelBuilder.Configurations.Add(new SecurityModelConfiguration());
            modelBuilder.Configurations.Add(new EquityModelConfiguration());
            modelBuilder.Configurations.Add(new InvestmentFundClassModelConfiguration());
            modelBuilder.Configurations.Add(new DebtModelConfiguration());
            modelBuilder.Configurations.Add(new SecuritiesPricingModelConfiguration());
            modelBuilder.Configurations.Add(new CapitalModelConfiguration());
            modelBuilder.Configurations.Add(new ValuationModelConfiguration());

            modelBuilder.Configurations.Add(new RatingModelConfiguration());
            modelBuilder.Configurations.Add(new RoleModelConfiguration());
            modelBuilder.Configurations.Add(new AssetClassificationModelConfiguration());
            modelBuilder.Configurations.Add(new CurrencyExchangeModelConfiguration());
            
            //modelBuilder.Configurations.Add(new Security_TESTModelConfiguration());
            //modelBuilder.Configurations.Add(new Asset_TESTModelConfiguration());
            
//            modelBuilder.ComplexType<FrequencyCode>().Property(p => p.IndexFrequency).HasColumnName("IndexFrequency");
#if !DEBUG
            // supprimer la table EdmMetaData
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
#endif


            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Surcharge de la methode de saveChanges
        /// </summary>
        /// <param name="context"></param>
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation", failure.Entry.Entity.GetType());
                    sb.AppendLine();
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:" + 
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
        }


        private ObjectContext ObjectContext()
        {
            return (this as IObjectContextAdapter).ObjectContext;
        }

        public int ExecuteSql(string sql)
        {
            int nbRows = -1;
            EntityConnection entityConnection = (EntityConnection) ObjectContext().Connection;
            DbConnection conn = entityConnection.StoreConnection;
            ConnectionState initialState = conn.State;
            try
            {
                if (initialState != ConnectionState.Open)
                    conn.Open();  // open connection if not already open
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    nbRows = cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                    conn.Close(); // only close connection if not initially open
            }
            return nbRows;
        }

        public IDataReader ExecuteSelectSql(string sql, params KeyValuePair<string,object>[] p)
        {
            IDataReader datareader = null;
            EntityConnection entityConnection = (EntityConnection)ObjectContext().Connection;
            IDbConnection conn = entityConnection.StoreConnection;
            ConnectionState initialState = conn.State;
            //try
            //{
                if (initialState != ConnectionState.Open)
                    conn.Open();  // open connection if not already open
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    foreach (KeyValuePair<string, object> parameter in p)
                    {
                        IDataParameter dbPar = cmd.CreateParameter();
                        dbPar.ParameterName = parameter.Key;
                        dbPar.Value = parameter.Value;
                        cmd.Parameters.Add(dbPar); 
                    }
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 60 * 10;
                    datareader= cmd.ExecuteReader();                    
                }
            //}
            //finally
            //{
            //    if (initialState != ConnectionState.Open)
            //        conn.Close(); // only close connection if not initially open
            //}
            return datareader;
        }


    }



}
