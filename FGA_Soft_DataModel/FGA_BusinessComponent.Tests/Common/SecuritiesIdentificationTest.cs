using FGABusinessComponent.BusinessComponent.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FGABusinessComponent.BusinessComponent.Security;

namespace FGA_BusinessComponent.Tests.Common
{
    
    
    /// <summary>
    ///Classe de test pour SecuritiesIdentificationTest, destinée à contenir tous
    ///les tests unitaires SecuritiesIdentificationTest
    ///</summary>
    [TestClass()]
    public class SecuritiesIdentificationTest
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
        ///Test pour Constructeur SecuritiesIdentification
        ///</summary>
        [TestMethod()]
        public void SecuritiesIdentificationConstructorTest()
        {
            string DomesticIdentificationSource = string.Empty; // TODO: initialisez à une valeur appropriée
            string Isin = "XX1234567890";
            string OtherIdentification = string.Empty; // TODO: initialisez à une valeur appropriée
            string ProprietaryIdentificationSource = string.Empty; // TODO: initialisez à une valeur appropriée
            string Bloomberg = string.Empty; // TODO: initialisez à une valeur appropriée
            string RIC = string.Empty; // TODO: initialisez à une valeur appropriée
            string SEDOL = string.Empty; // TODO: initialisez à une valeur appropriée
            SecuritiesIdentification target = new SecuritiesIdentification(DomesticIdentificationSource, Isin, OtherIdentification, ProprietaryIdentificationSource, Bloomberg, RIC, SEDOL);
            if( target.SecurityIdentification == "XX1234567890" ){
                Assert.IsTrue(true);
            }
        }


        /// <summary>
        ///Test pour Bloomberg
        ///</summary>
        [TestMethod()]
        public void BloombergTest()
        {
            SecuritiesIdentification target = new SecuritiesIdentification(); // TODO: initialisez à une valeur appropriée
            BloombergIdentifier expected = null; // TODO: initialisez à une valeur appropriée
            BloombergIdentifier actual;
            target.Bloomberg = expected;
            actual = target.Bloomberg;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour DomesticIdentificationSource
        ///</summary>
        [TestMethod()]
        public void DomesticIdentificationSourceTest()
        {
            SecuritiesIdentification target = new SecuritiesIdentification(); // TODO: initialisez à une valeur appropriée
            CountryCode expected = null; // TODO: initialisez à une valeur appropriée
            CountryCode actual;
            target.DomesticIdentificationSource = expected;
            actual = target.DomesticIdentificationSource;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour Id
        ///</summary>
        [TestMethod()]
        public void IdTest()
        {
            SecuritiesIdentification target = new SecuritiesIdentification(); // TODO: initialisez à une valeur appropriée
            long expected = 0; // TODO: initialisez à une valeur appropriée
            long actual;
            target.Id = expected;
            actual = target.Id;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour OtherIdentification
        ///</summary>
        [TestMethod()]
        public void OtherIdentificationTest()
        {
            SecuritiesIdentification target = new SecuritiesIdentification(); // TODO: initialisez à une valeur appropriée
            string expected = string.Empty; // TODO: initialisez à une valeur appropriée
            string actual;
            target.OtherIdentification = expected;
            actual = target.OtherIdentification;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour ProprietaryIdentificationSource
        ///</summary>
        [TestMethod()]
        public void ProprietaryIdentificationSourceTest()
        {
            SecuritiesIdentification target = new SecuritiesIdentification(); // TODO: initialisez à une valeur appropriée
            string expected = string.Empty; // TODO: initialisez à une valeur appropriée
            string actual;
            target.ProprietaryIdentificationSource = expected;
            actual = target.ProprietaryIdentificationSource;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour RIC
        ///</summary>
        [TestMethod()]
        public void RICTest()
        {
            SecuritiesIdentification target = new SecuritiesIdentification(); // TODO: initialisez à une valeur appropriée
            RICIdentifier expected = null; // TODO: initialisez à une valeur appropriée
            RICIdentifier actual;
            target.RIC = expected;
            actual = target.RIC;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour SEDOL
        ///</summary>
        [TestMethod()]
        public void SEDOLTest()
        {
            SecuritiesIdentification target = new SecuritiesIdentification(); // TODO: initialisez à une valeur appropriée
            SEDOLIdentifier expected = null; // TODO: initialisez à une valeur appropriée
            SEDOLIdentifier actual;
            target.SEDOL = expected;
            actual = target.SEDOL;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour SecurityIdentification
        ///</summary>
        [TestMethod()]
        public void SecurityIdentificationTest()
        {
            SecuritiesIdentification target = new SecuritiesIdentification(); // TODO: initialisez à une valeur appropriée
            ISINIdentifier expected = null; // TODO: initialisez à une valeur appropriée
            ISINIdentifier actual;
            target.SecurityIdentification = expected;
            actual = target.SecurityIdentification;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }
    }
}
