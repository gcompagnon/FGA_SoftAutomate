using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using FileHelpers.RunTime;
using System.IO;

namespace ReactiveETL.Tests.PortfolioHoldings
{
    /// <summary>
    /// Tests intégration de ReactiveETL et Automapper
    /// </summary>
    [TestClass]
    public class MapperTests
    {
        public MapperTests()
        {
            //
            // TODO: ajoutez ici la logique du constructeur
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Obtient ou définit le contexte de test qui fournit
        ///des informations sur la série de tests active ainsi que ses fonctionnalités.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Attributs de tests supplémentaires
        //
        // Vous pouvez utiliser les attributs supplémentaires suivants lorsque vous écrivez vos tests :
        //
        // Utilisez ClassInitialize pour exécuter du code avant d'exécuter le premier test de la classe
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Utilisez ClassCleanup pour exécuter du code une fois que tous les tests d'une classe ont été exécutés
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Utilisez TestInitialize pour exécuter du code avant d'exécuter chaque test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Utilisez TestCleanup pour exécuter du code après que chaque test a été exécuté
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region classes de données DTO

        public class CalendarEvent
        {
            public DateTime EventDate { get; set; }
            public string Title { get; set; }
        }
        public class CalendarEventForm
        {
            public DateTime EventDate { get; set; }
            public int EventHour { get; set; }
            public int EventMinute { get; set; }
            public string Title { get; set; }
        }
        #endregion

        const string ROOT = @"C:\FGA_SOFT\DEVELOPPEMENT\DATA\PORTFOLIO\";

        [TestMethod]
        public void AutomapperOnly()
        {
            // Model
            var calendarEvent = new CalendarEvent
            {
                EventDate = new DateTime(2008, 12, 15, 20, 30, 0),
                Title = "Company Holiday Party"
            };

            // Configure AutoMapper
            Mapper.CreateMap<CalendarEvent, CalendarEventForm>()
                .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.EventDate.Date))
                .ForMember(dest => dest.EventHour, opt => opt.MapFrom(src => src.EventDate.Hour))
                .ForMember(dest => dest.EventMinute, opt => opt.MapFrom(src => src.EventDate.Minute));

            // Perform mapping
            CalendarEventForm form = Mapper.Map<CalendarEvent, CalendarEventForm>(calendarEvent);

            Assert.AreEqual<DateTime>(new DateTime(2008, 12, 15), form.EventDate);
            Assert.AreEqual<int>(20, form.EventHour);
            Assert.AreEqual<int>(30, form.EventMinute);
            Assert.AreEqual<string>("Company Holiday Party", form.Title);

        }

        #region classes de données DTO
        public class Indextest
        {
            public string CompoISIN { get; set; }
            public decimal CompoWeight { get; set; }
        }

        static List<Indextest> allIndex = new List<Indextest>();
        #endregion

        [TestMethod]
        public void MapperOperation()
        {

            string fileName = "PortfolioHolding_CLICHY_P.csv";

            // classe dynamique generee par un fichier xml
            ClassBuilder cd = ClassBuilder.LoadFromXml(ROOT + "PortfolioHolding.fhw");
            Type recordClass = cd.CreateRecordClass();

            // // Configure AutoMapper
            Mapper.CreateMap(recordClass, typeof(Indextest));

            using (System.IO.StreamReader file = new StreamReader(ROOT + fileName))
            {
                // string content = file.ReadToEnd();
                file.BaseStream.Position = 1;
                var filecontent = Input
                    .ReadFile(file, recordClass)
                    .Mapper(recordClass,typeof(Indextest))
                    .Execute();
            }

        }
    }
}
