using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGA.Automate.Helpers
{
    public class Helper
    {
        static ConvertHelpers.DecimalConverter mDecimalConv;
        static ConvertHelpers.DoubleConverter mDoubleConv;
        static ConvertHelpers.SingleConverter mSingleConv;
        static ConvertHelpers.DateTimeConverter mDateConv;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ValueToString(object o)
        {
            if (mDecimalConv == null)
            {
                mDecimalConv = new ConvertHelpers.DecimalConverter(".");
                mDoubleConv = new ConvertHelpers.DoubleConverter(".");
                mSingleConv = new ConvertHelpers.SingleConverter(".");
                mDateConv = new ConvertHelpers.DateTimeConverter("dd/MM/yyyy");
            }

            if (o == null)
                return string.Empty;
            else if (o is DateTime)
                return mDateConv.FieldToString(o);
            else if (o is Decimal)
                return mDecimalConv.FieldToString(o);
            else if (o is Double)
                return mDoubleConv.FieldToString(o);
            else if (o is Single)
                return mSingleConv.FieldToString(o);
            else
                return o.ToString();

        }
        /// <summary>
        /// Utilitaire pour formater une valeur en champ / valeur de type SQL
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ValueToSQLField(object o)
        {
            if (mDecimalConv == null)
            {
                mDecimalConv = new ConvertHelpers.DecimalConverter(".");
                mDoubleConv = new ConvertHelpers.DoubleConverter(".");
                mSingleConv = new ConvertHelpers.SingleConverter(".");
                mDateConv = new ConvertHelpers.DateTimeConverter("dd/MM/yyyy");
            }

            if (o == null)
                return "'" + string.Empty + "'";
            else if (o is DateTime)
                return "'" + mDateConv.FieldToString(o) + "'";
            else if (o is Decimal)
                return mDecimalConv.FieldToString(o);
            else if (o is Double)
                return mDoubleConv.FieldToString(o);
            else if (o is Single)
                return mSingleConv.FieldToString(o);
            else
                return "'" + o.ToString() + "'";

        }

    }
}
