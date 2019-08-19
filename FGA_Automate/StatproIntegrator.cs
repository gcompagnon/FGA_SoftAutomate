using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Config;
using log4net;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using System.Data;
using System.Collections.Specialized;
using FGA.Automate.Consumer;


namespace FGA.Automate.Statpro
{



    /// <summary>
    /// Classe de lecture des fichiers Statpro
    /// </summary>
    class StatproProducer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subIndice">le code du subindice</param>
        /// <param name="isin"></param>
        /// <param name="lines">la structure : subindices -> (isin -> line de param ) </param>
        /// <param name="compo">la structure : isin -> list des subindices contenant l isin</param>
        /// <param name="fields"></param>
        private static void Enrich(string subIndice, string isin, IDictionary<String, IDictionary<String, String>> lines, IDictionary<String, StringCollection> compo, object[] fields)
        {
            IDictionary<String, String> subIndiceCompo;
            // Recherche si il y a déjà la compo du sous-indice
            if (!lines.TryGetValue(subIndice, out subIndiceCompo))
            {
                subIndiceCompo = new Dictionary<String, String>();
                lines.Add(subIndice, subIndiceCompo);
            }

            string line;
            // Recherche si l isin est dans ce sous indice
            if (!subIndiceCompo.TryGetValue(isin, out line))
            {
                line = "{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16}";                
            }
            // enrichir la ligne avec les données fields
            line = String.Format(line, fields);
            subIndiceCompo[isin] = line;
//            subIndiceCompo.Add(isin, line);
            
            if (compo != null)
            {
                //Recherche si l isin fait parti de la collection des valeurs / et des sous indices
                StringCollection SubIndices;
                if (!compo.TryGetValue(isin, out SubIndices))
                {
                    SubIndices = new StringCollection();
                    compo.Add(isin, SubIndices);
                }
                SubIndices.Add(subIndice);
            }
        }

        ///<summary>
        ///Lecture du contenu du fichier texte
        ///</summary>
        ///<param name=fileName> nom du fichier sans chemin</param>
        ///<returns>contenu du fichier</returns>
        public static void readFileEMTX(string indexFamily,string extension, string date, string file, IDictionary<String, IDictionary<String, String>> lines, IDictionary<String, StringCollection> compo)
        {
            StringCollection sc = new StringCollection();
            string isin = null;
            CsvReader csv = null;
            string paysf;
            string soussecteurf;
            string ratingf;
            string secteurf;
            try
            {
                string path = @"C:\DATA\STATPRO\";
                csv = new CsvReader(new StreamReader(path + file), true, ';');

                int fieldCount = csv.FieldCount;
                int lineCount = 0;

                string[] headers = csv.GetFieldHeaders();

                while (csv.ReadNextRecord())
                {
                    lineCount++;
                    if (lineCount == 1)
                        continue;
                    if (extension == "cn2" & csv.FieldCount >= 28)
                    {
                        isin = csv[3];
                        string subIndice = csv[1];
                        double poids = double.Parse(csv[18]) /100;

                        object[] fields = { 
                                csv[12], //0. date de l indice
                                csv[1],  //1. code de l indice
                                "{2}",
                                csv[3],  //3. code titre ( il y a un isin corrigé par Statpro en position 8 : mais vu une erreur)
                                csv[11], //4. libellé titre 
                                poids, //5. poids (euromts)
                                "{6}",
                                "{7}",
                                csv[28], //8. devise du titre (prix)
                                "{9}","{10}","{11}","{12}","{13}","{14}","{15}","{16}"
                                          };
                        Enrich(subIndice, isin, lines, compo, fields);
                    }
                    else if (extension == "rns" & csv.FieldCount >= 23 & indexFamily == "EMTS")
                    {
                        isin = csv[1];
                        object[] fields = { 
                                "{0}","{1}","{2}","{3}","{4}","{5}","{6}","{7}","{8}","{9}","{10}",
                                "{11}","{12}","{13}","{14}","{15}",
                                csv[14] // 16. duration (risk number 1)
                                };

                        foreach (string subIndice in compo[isin])
                        {
                            Enrich(subIndice, isin, lines, null, fields);
                        }
                    }
                    else if (extension == "sec" & csv.FieldCount >= 23 & indexFamily == "EMTS")
                    {
                        isin = csv[1];
                        DateTime dateMat = DateTime.Parse(csv[32]);
                        
                        if (!StatproIntegrator.PAYS.TryGetValue(csv[14].ToUpper(), out paysf))
                            paysf = csv[14];
                        
                        if (!StatproIntegrator.SOUSSECTEUR.TryGetValue(paysf, out soussecteurf))
                            soussecteurf = "";
                        
                        if (!StatproIntegrator.RATING.TryGetValue(paysf, out ratingf))
                            ratingf = "";                        
                        object[] fields = { 
                                "{0}","{1}","{2}","{3}","{4}","{5}",
                                0, // poids cc
                                "Obligations Taux Fixe",
                                "{8}",
                                "EMPRUNTS D'ETAT",
                                soussecteurf,
                                paysf,paysf, // pays , emetteur 
                                ratingf,paysf, // grp emetteur
                                dateMat.ToShortDateString(), // maturité
                                "" 
                                };

                        foreach (string subIndice in compo[isin])
                        {
                            Enrich(subIndice, isin, lines, null, fields);
                        }
                    }
                    else if (extension == "sec" & csv.FieldCount >= 23 & indexFamily == "STOX")
                    {
                        isin = csv[1];                        
                        if (!StatproIntegrator.PAYS.TryGetValue(csv[14].ToUpper(), out paysf))
                            paysf = csv[14];                        
                        if (!StatproIntegrator.SOUSSECTEURICB.TryGetValue(csv[26].ToUpper(), out soussecteurf))
                            soussecteurf = "";
                        
                        if (!StatproIntegrator.SECTEURICB.TryGetValue(csv[24].ToUpper(), out secteurf))
                            secteurf = "";

                        string type;
                        if (!StatproIntegrator.ACTIONSTYPE.TryGetValue(paysf, out type))
                            type = "Action";

                        string emetteur = csv[9].ToUpper();
                        object[] fields = { 
                                "{0}","{1}","{2}","{3}","{4}","{5}",
                                0, // poids cc
                                type,
                                "{8}",
                                secteurf,
                                soussecteurf,
                                paysf,emetteur, // pays , emetteur 
                                "",emetteur, // grp emetteur
                                "",
                                "" 
                                };

                        foreach (string subIndice in compo[isin])
                        {
                            Enrich(subIndice, isin, lines, null, fields);
                        }
                    }
                    else if (extension == "ser" & csv.FieldCount >= 16)
                    {
                        string subIndice = csv[1];
                        object[] fields = { 
                                "{0}","{1}",csv[15], // nom de la serie
                                "{3}","{4}","{5}","{6}","{7}","{8}","{9}","{10}","{11}","{12}","{13}","{14}","{15}","{16}"
                                          };
                        IDictionary<String,String> compoI = lines[subIndice];
                        String[] isins = compoI.Keys.ToArray<String>();
                        foreach (string v_isin in isins)
                        {
                            Enrich(subIndice, v_isin, lines, null, fields);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Erreur: \nFichier texte: " + file + " : "+isin, e);
            }
        }
    }


    /// <summary>
    /// Classe pour la création/ maj des Indices 
    /// </summary>
    class IndexBusinessComponentConsumer
    {

    }


    public class StatproIntegrator
    {
        public static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            string[] families = { "EMTS", "ENX", "IBOX", "STOX" };
            string[] fileType = { "ser", "sch", "scc", "rns", "mks", "exs", "ech", "cn2" };

            string staticData = "SECURITY";
            string[] fileType2 = { "ser", "sch", "scc", "rns", "mks", "exs", "ech", "cn2" };
            //                                            x             z      y      y

            // Algorithme :
            // Pour une famille d indice : un code

            // Pour chaque date présente: il y a un fichier MMJJAAAA.xxx et un répertoire MMJJAAAA
            //string dateMD = "03302012";
            //string dateImportation = "20120331";
            //string indexFamily = "EMTS";

            //string dateMD = "04302012";
            string dateMD = "09302014";
            string dateImportation = "20141001";
            //string indexFamily = "STOX";
            string indexFamily = "EMTS";
            //string indexFamily = "IBOX";
            
            IDictionary<String, IDictionary<String, String>> lines = new Dictionary<String, IDictionary<String, String>>();
            IDictionary<String, StringCollection> compo = new Dictionary<String, StringCollection>();
            
            string extension = "cn2";
            string relativePath = dateImportation + "\\Renamed\\" + indexFamily + "\\" + dateMD + "\\" + indexFamily + '.' + extension;
            StatproProducer.readFileEMTX(indexFamily,extension, dateMD, relativePath, lines, compo);

            extension = "rns";
            relativePath = dateImportation + "\\Renamed\\" + indexFamily + "\\" + dateMD + "\\" + indexFamily + '.' + extension;
            StatproProducer.readFileEMTX(indexFamily,extension, dateMD, relativePath, lines, compo);

            extension = "sec";
            relativePath = dateImportation + "\\Renamed\\" + indexFamily + "\\SECURITY\\SECURITY.sec";
            StatproProducer.readFileEMTX(indexFamily,extension, dateMD, relativePath, lines, compo);

            extension = "ser";
            relativePath = dateImportation + "\\Renamed\\" + indexFamily + "\\" + dateMD + "\\" + indexFamily + '.' + extension;
            StatproProducer.readFileEMTX(indexFamily,extension, dateMD, relativePath, lines, compo);




            //foreach (string indexFamily in families)
            //{
            //    foreach (string extension in fileType)
            //    {
            //        string relativePath = dateImportation + "\\Renamed\\" + indexFamily + "\\" + dateMD + "\\" + indexFamily + '.' + extension;
            //        StatproProducer.readFile(indexFamily, dateMD, relativePath);
            //    }
            //}


            // Sortie fichier de proxy
            ProxyText producerText = new ProxyText();
            //            string bp2sValid = (CommandLine["bp2sregex"] == null ? @"Resources/Consumer/BP2StextValidation.txt" : CommandLine["bp2sregex"]);
            //            InfoLogger.Debug("Lecture d un fichier de validation des champs (regex) " + bp2sValid);

            //string bp2s = CommandLine["bp2s"];
            //bp2stext.ValidationPath = bp2sValid;

            //InfoLogger.Debug("Configuration pour une sortie BP2S " + bp2s);
            StringCollection sc ;
            sc = new StringCollection();

            foreach (string subIndice in lines.Keys)
            {
                IDictionary<String, String> compoI = lines[subIndice];
                foreach (string isin in compoI.Keys)
                {
                    
                    string line = compoI[isin];
                    sc.Add(line);
                }
                
            }
            string file = @"G:\,FGA Soft\BASE\REF\PROXY\Proxy" + indexFamily + "_YYYYMMDD.csv";
            file = file.Replace("YYYYMMDD", now.ToString("yyyyMMdd"));

            producerText.CreateFile(sc, file, now);

            //string file = @"G:\,FGA Soft\SQLLauncher\BP2S\Proxy_YYYYMMDD.csv";
            //file = file.Replace("YYYYMMDD", now.ToString("yyyyMMdd"));

            //producerText.CreateFile(sc, file, now);

        }


        static public Dictionary<String, string> PAYS;
        static public Dictionary<String, string> SOUSSECTEUR;
        static public Dictionary<String, string> SOUSSECTEURICB;
        static public Dictionary<String, string> SECTEURICB;
        static public Dictionary<String, string> RATING;
        static public Dictionary<String, string> ACTIONSTYPE;
        static StatproIntegrator()
    {
        PAYS = new Dictionary<String, string>();
        SOUSSECTEURICB = new Dictionary<String, string>();
        SECTEURICB = new Dictionary<String, string>();
        SOUSSECTEUR = new Dictionary<String, string>();
        RATING = new Dictionary<String, string>();
        ACTIONSTYPE = new Dictionary<String, string>();
        PAYS.Add("AUSTRIA", "Autriche");
        PAYS.Add("AUSTRALIA", "Australie");
        PAYS.Add("BERMUDA", "Bermudes");
        PAYS.Add("BELGIUM", "Belgique");
        PAYS.Add("BAHAMAS", "Bahamas");
        PAYS.Add("BRAZIL", "Brésil");
        PAYS.Add("CANADA", "Canada");
        PAYS.Add("SWITZERLAND", "Suisse");
        PAYS.Add("CYPRUS", "Chypre");
        PAYS.Add("CZECH REPUBLIC", "République Tchèque");
        PAYS.Add("GERMANY", "Allemagne");
        PAYS.Add("DENMARK", "Danemark");
        PAYS.Add("SPAIN", "Espagne");
        PAYS.Add("FINLAND", "Finlande");
        PAYS.Add("FRANCE", "France");
        PAYS.Add("UNITED KINGDOM", "Royaume-Uni");
        PAYS.Add("GREECE", "Grèce");
        PAYS.Add("GUERNSEY", "Guernesey");
        PAYS.Add("HONG KONG", "Hong-Kong");
        PAYS.Add("HUNGARY", "Hongrie");
        PAYS.Add("INDONESIA", "Indonésie");
        PAYS.Add("IRELAND", "Irlande");
        PAYS.Add("ICELAND", "Islande");
        PAYS.Add("ITALY", "Italie");
        PAYS.Add("JERSEY", "Jersey");
        PAYS.Add("JAPAN", "Japon");
        PAYS.Add("SOUTH KOREA", "Corée du Sud");
        PAYS.Add("CAYMAN ISLANDS", "Iles Caiman");
        PAYS.Add("LITUANIA", "Lituanie");
        PAYS.Add("LUXEMBOURG", "Luxembourg");
        PAYS.Add("MOROCCO", "MAROC");
        PAYS.Add("WORLD", "Monde");
        PAYS.Add("MEXICO", "Mexique");
        PAYS.Add("MALAYSIA", "Malaisie");
        PAYS.Add("Not Available", "Not Available");
        PAYS.Add("NETHERLANDS ANTILLES", "Antilles Hollandaises");
        PAYS.Add("NETHERLANDS", "Pays-Bas");
        PAYS.Add("NORWAY", "Norvège");
        PAYS.Add("NEW ZEALAND", "Nouvelle Zélande");
        PAYS.Add("PANAMA", "Panama");
        PAYS.Add("POLAND", "Pologne");
        PAYS.Add("PORTUGAL", "Portugal");
        PAYS.Add("SWEDEN", "Suède");
        PAYS.Add("SINGAPORE", "Singapour");
        PAYS.Add("SLOVAKIA", "Slovaquie");
        PAYS.Add("SUPRANATIONAL", "Supranational");
        PAYS.Add("THAILANDE", "Thailande");
        PAYS.Add("USA", "Etats-Unis");
        PAYS.Add("VIRGIN ISLANDS U.K.", "Iles Vierges Britanniques");
        PAYS.Add("CLEARSTREAM", "Clearstream");
        PAYS.Add("EUROCLEAR BANK", "Euroclear Bank");
        PAYS.Add("SOUTH AFRICA", "Afrique du Sud");
        PAYS.Add("NORTH AMERICA", "Zone  Amerique Nord");
        PAYS.Add("INDIA", "Zone  Asie");
        PAYS.Add("EUROPE", "Zone  Europe");
        PAYS.Add("UNITED ARAB EMIRATES", "Zone Émirats arabes unis");
        PAYS.Add("ZIMBABWE", "Zimbabwe");


        SOUSSECTEUR.Add("Espagne", "EMPRUNT D'ETAT ESPAGNOL");
        SOUSSECTEUR.Add("Allemagne", "EMPRUNT D'ETAT ALLEMAND");
        SOUSSECTEUR.Add("Italie", "EMPRUNT D'ETAT ITALIEN");
        SOUSSECTEUR.Add("Euro", "EMPRUNTS D'ETAT EURO");
        SOUSSECTEUR.Add("Pays-Bas", "EMPRUNT D'ETAT HOLLANDAIS");
        SOUSSECTEUR.Add("Portugal", "EMPRUNT D'ETAT PORTUGAIS");
        SOUSSECTEUR.Add("Grèce", "EMPRUNT D'ETAT GREC");
        SOUSSECTEUR.Add("Finlande", "EMPRUNT D'ETAT FINLANDAIS");
        SOUSSECTEUR.Add("France", "EMPRUNT D'ETAT FRANCAIS");
        SOUSSECTEUR.Add("Autriche", "EMPRUNT D'ETAT AUTRICHIEN");
        SOUSSECTEUR.Add("Belgique", "EMPRUNT D'ETAT BELGE");
        SOUSSECTEUR.Add("Irlande", "EMPRUNT D'ETAT IRLANDAIS");

        RATING.Add("Allemagne", "AAA");
        RATING.Add("Autriche", "AA+");
        RATING.Add("Belgique", "AA-");
        RATING.Add("Espagne", "BBB-");
        RATING.Add("Finlande", "AAA");
        RATING.Add("France", "AA+");
        RATING.Add("Grèce", "CCC");
        RATING.Add("Irlande", "BB+");
        RATING.Add("Italie", "BBB");
        RATING.Add("Pays-Bas", "AAA");
        RATING.Add("Portugal", "BB-");

        SOUSSECTEURICB.Add("3500", "ALIMENTAIRE - BOISSON");
        SOUSSECTEURICB.Add("3300", "AUTOMOBILE");
        SOUSSECTEURICB.Add("3700", "B&S DE CONSOMMATION");
        SOUSSECTEURICB.Add("2700", "B&S INDUSTRIELS");
        SOUSSECTEURICB.Add("2300", "CONSTRUCTION");
        SOUSSECTEURICB.Add("1300", "CHIMIE");
        SOUSSECTEURICB.Add("1700", "PRODUITS DE BASE");
        SOUSSECTEURICB.Add("0500", "PETROLE ET GAZ");
        SOUSSECTEURICB.Add("4500", "SANTE");
        SOUSSECTEURICB.Add("7500", "SERVICES AUX COLLECTIVITES");
        SOUSSECTEURICB.Add("5300", "DISTRIBUTION");
        SOUSSECTEURICB.Add("5500", "MEDIAS");
        SOUSSECTEURICB.Add("5700", "VOYAGES ET LOISIRS");
        SOUSSECTEURICB.Add("8500", "ASSURANCE");
        SOUSSECTEURICB.Add("8300", "BANQUES");
        SOUSSECTEURICB.Add("8600", "IMMOBILIER");
        SOUSSECTEURICB.Add("8700", "SERVICES FINANCIERS");
        SOUSSECTEURICB.Add("9500", "TECHNOLOGIE");
        SOUSSECTEURICB.Add("6500", "TELECOMMUNICATIONS");


        SECTEURICB.Add("3000", "BIENS DE CONSOMMATION");
        SECTEURICB.Add("2000", "INDUSTRIES");
        SECTEURICB.Add("1000", "MATERIAUX DE BASE");
        SECTEURICB.Add("0001", "PETROLE ET GAZ");
        SECTEURICB.Add("4000", "SANTE");
        SECTEURICB.Add("7000", "SERVICES AUX COLLECTIVITES");
        SECTEURICB.Add("5000", "SERVICES AUX CONSOMMATEURS");
        SECTEURICB.Add("8000", "SOCIETES FINANCIERES");
        SECTEURICB.Add("9000", "TECHNOLOGIE");
        SECTEURICB.Add("6000", "TELECOMMUNICATIONS");

        ACTIONSTYPE.Add("Allemagne", "Actions Allemagne");
        ACTIONSTYPE.Add("Autriche", "Actions Autriche");
        ACTIONSTYPE.Add("Belgique", "Actions Belgique");
        ACTIONSTYPE.Add("Canada", "Actions Canada");
        ACTIONSTYPE.Add("Danemark", "Actions Danemark");
        ACTIONSTYPE.Add("Espagne", "Actions Espagne");
        ACTIONSTYPE.Add("Finlande", "Actions Finlande");
        ACTIONSTYPE.Add("France", "Actions France");
        ACTIONSTYPE.Add("Royaume-Uni", "Actions GB");
        ACTIONSTYPE.Add("Grèce", "Actions Grèce");
        ACTIONSTYPE.Add("Irlande", "Actions Irlande");
        ACTIONSTYPE.Add("Italie", "Actions Italie");
        ACTIONSTYPE.Add("Japon", "Actions Japon");
        ACTIONSTYPE.Add("Luxembourg", "Actions Luxembourg");
        ACTIONSTYPE.Add("Norvège", "Actions Norvège");
        ACTIONSTYPE.Add("Pays-Bas", "Actions Pays-bas");
        ACTIONSTYPE.Add("Portugal", "Actions Portugal");
        ACTIONSTYPE.Add("Suède", "Actions Suède");
        ACTIONSTYPE.Add("Suisse", "Actions Suisse");
        ACTIONSTYPE.Add("République Tchèque", "Actions République Tchèque");



    }

    }
}
