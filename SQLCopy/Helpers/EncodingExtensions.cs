using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLCopy.Helpers
{
    /// <summary>
    /// Extension of existing Encoding but with String operator
    /// </summary>
    public static class EncodingExtensions
    {
     
        /// <summary>
        /// Extension to get a string.GetEncoding() returns a Encoding object
        /// </summary>
        /// <param name="csvFileEncoding"></param>
        /// <returns></returns>
        public static Encoding GetEncoding(this string csvFileEncoding)
        {
            Encoding enc = null;
            switch (csvFileEncoding)
            {
                case "UTF8": enc = Encoding.UTF8; break;
                case "ASCII": enc = Encoding.ASCII; break;
                case "BigEndianUnicode": enc = Encoding.BigEndianUnicode; break;
                case "Unicode": enc = Encoding.Unicode; break;
                case "UTF32": enc = Encoding.UTF32; break;
                case "UTF7": enc = Encoding.UTF7; break;
                default: enc = Encoding.Default; break;
            };
            return  enc;
        }

    }
}
