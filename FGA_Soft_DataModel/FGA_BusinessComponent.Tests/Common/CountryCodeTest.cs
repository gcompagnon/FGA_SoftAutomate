using FGABusinessComponent.BusinessComponent.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FGA_BusinessComponent.Tests
{
    
    
    /// <summary>
    ///Classe de test pour CountryCodeTest, destinée à contenir tous
    ///les tests unitaires CountryCodeTest
    ///</summary>
    [TestClass()]
    public class CountryCodeTest
    {


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
        //Vous pouvez utiliser les attributs supplémentaires suivants lorsque vous écrivez vos tests :
        //
        //Utilisez ClassInitialize pour exécuter du code avant d'exécuter le premier test dans la classe
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Utilisez ClassCleanup pour exécuter du code après que tous les tests ont été exécutés dans une classe
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Utilisez TestInitialize pour exécuter du code avant d'exécuter chaque test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Utilisez TestCleanup pour exécuter du code après que chaque test a été exécuté
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Test pour Constructeur CountryCode
        ///</summary>
        [TestMethod()]
        public void CountryCodeConstructorTest()
        {
            CountryCodeAdaptee Instance = new CountryCodeAdaptee("YY", "TEST", 10, "Pays Test", "TEST");
            CountryCode target = new CountryCode(Instance);
            Assert.AreEqual("TEST", target.ToString());
        }


        /// <summary>
        ///Test pour ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            CountryCode target = (CountryCode)"XX";
            string expected = "INCONNU";
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);         
        }

        /// <summary>
        ///Test pour getCountryByLabel
        ///</summary>
        [TestMethod()]
        public void getCountryByLabelTest()
        {
            string label = "INCONNU";
            CountryCode expected = CountryCode.DEFAULT;
            CountryCode actual;
            actual = CountryCode.getCountryByLabel(label);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Test pour op_Explicit
        ///</summary>
        [TestMethod()]
        public void op_ExplicitTest()
        {
            string str = "XX";
            CountryCode expected = CountryCode.DEFAULT;
            CountryCode actual;
            actual = ((CountryCode)(str));
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Test pour Country
        ///</summary>
        [TestMethod()]
        public void CountryTest()
        {
            CountryCode target = (CountryCode)"XX";
            string expected = "XX"; 
            string actual;
            target.Country = expected;
            actual = target.Country;
            Assert.AreEqual(expected, actual);
        }
    }
}
