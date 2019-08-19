using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Integrator.Config
{
    /// <summary>
    /// Classe utilitaire pour représenter un fichier XML (de configuration)
    /// </summary>
    class XMLFile
    {
        public static XMLFile LoadXMLFile(string path)
        {
            XDocument fromFile = XDocument.Load("@" + path);
            return null;
        }


    }
}
