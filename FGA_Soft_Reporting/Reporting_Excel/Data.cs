using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reporting_Excel
{
    class ChartData
    {
        public string titre;
        public string nomModele;
        public List<int> data;
        public string colValeurs;
        public string colTitres;

        public ChartData(string titr, string nomModele, List<int> dat, string colVal, string colTit)
        {
            this.titre = titr;
            this.nomModele = nomModele;
            data = dat;
            colValeurs = colVal;
            colTitres = colTit;
        }
    }

    class MetaChartData
    {
        public string titre;
        public string nomModele;
        public int indice;
        public List<string> series;
        public bool transpose;

        public MetaChartData(string titr, string nomMod, int ind, List<string> ser)
        {
            this.titre = titr;
            nomModele = nomMod;
            indice = ind;
            series = ser;
            transpose = false;
        }

        public MetaChartData(string titr, string nomMod, int ind, List<string> ser,bool trans)
        {
            this.titre = titr;
            nomModele = nomMod;
            indice = ind;
            series = ser;
            transpose = trans;
        }
    }

    class TableauData
    {
        public int indice;

        public TableauData(int ind)
        {
            indice = ind;
        }
    }
}
