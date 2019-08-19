using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace Helpers.DataReader
{
    //TODO: implementer lDatareader pour XML
    /// <summary>
    /// DataReader Implementation for a XML File
    /// </summary>
    public class XMLFileDataReader : FileDataRecord, IDataReader
    {

        public XMLFileDataReader(Stream fileStream, FileDataColumn[] columns, char recordSeparator, char fieldSeparator, Encoding fileEncoding, Action<FileDataRecord> recordManipulator)
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

        public DataTable GetSchemaTable()
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
