using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers.DataReader;
using System.Data;
using System.IO;

namespace SQLCopy.Helpers.DataReader
{
    //TODO: implementer l adapter openxml 
    /// <summary>
    /// Datareader implementation based on the OpenXMLReader (Adapter pattern)
    /// </summary>
    public class OpenXMLDataReader: FileDataRecord, IDataReader
    {



        public OpenXMLDataReader(StreamReader fileStreamreader,FileDataColumn[] columns, bool includeFirstRowAsColumns, string worksheetName, string startCell,string endCell,  Action<FileDataRecord> recordManipulator)
            : base(columns)
        {
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public System.Data.DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool IsClosed
        {
            get { throw new NotImplementedException(); }
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            throw new NotImplementedException();
        }

        public int RecordsAffected
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


        public object this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public object this[int i]
        {
            get { throw new NotImplementedException(); }
        }
    }
}
