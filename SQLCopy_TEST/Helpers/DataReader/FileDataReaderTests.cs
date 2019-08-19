using System;
using System.Data;
using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Helpers.DataReader;

namespace FileDataReaderTests
{    
    [TestClass]
    public class FileDataReaderTests
    {

        [TestMethod]
        public void ReadShouldReturnTrueIfThereAreAValidRecord()
        {
            Stream s = new MemoryStream(1000);
            for (int x = 0; x < 1; x++)
            {
                AddRecordToStream(s, "10,2010-05-05 10:00");
            }
            s.Position = 0;
            FileDataColumn[] cols = new[] 
            { 
                new FileDataColumn { ColumnName = "First", ColumnType = typeof(int) }, 
                new FileDataColumn { ColumnName = "Second", ColumnType = typeof(DateTime) } 
            };

            IDataReader dataReader = new FileDataReader(s, cols, '\n', ',', Encoding.Unicode, null);

            bool hasRecords = dataReader.Read();
            Assert.IsTrue(hasRecords);

        }

                [TestMethod]
        public void DataShouldReflectTheValues()
        {
            Stream s = new MemoryStream(1000);
            for (int x = 0; x < 1; x++)
            {
                AddRecordToStream(s, "10,2010-05-05 10:00");
            }
            s.Position = 0;
            FileDataColumn[] cols = new[] 
            { 
                new FileDataColumn { ColumnName = "First", ColumnType = typeof(int) }, 
                new FileDataColumn { ColumnName = "Second", ColumnType = typeof(DateTime) } 
            };

            IDataReader dataReader = new FileDataReader(s, cols, '\n', ',', Encoding.Unicode, null);

            bool hasRecords = dataReader.Read();
            int first = (int)dataReader[0];
                    Assert.Equals(first, 10);
            DateTime second = (DateTime)dataReader[1];
                    Assert.Equals(second, new DateTime(2010, 5, 5, 10, 0, 0));

        }

                [TestMethod]
        public void DateTimeShouldBeSupported()
        {
            Stream s = new MemoryStream(1000);
            for (int x = 0; x < 1; x++)
            {
                AddRecordToStream(s, "2010-05-05 10:00:01.005");
            }
            s.Position = 0;
            FileDataColumn[] cols = new[] 
            { 
                new FileDataColumn { ColumnName = "First", ColumnType = typeof(DateTime) } 
            };

            IDataReader dataReader = new FileDataReader(s, cols, '\n', ',', Encoding.Unicode, null);

            dataReader.Read();
            DateTime dateTime = (DateTime)dataReader[0];
                    Assert.Equals(dateTime, new DateTime(2010, 5, 5, 10, 0, 1, 5));
        }

                [TestMethod]
        public void DoubleShouldBeSupported()
        {
            Stream s = new MemoryStream(1000);
            for (int x = 0; x < 1; x++)
            {
                AddRecordToStream(s, "10,0008\n");
            }
            s.Position = 0;
            FileDataColumn[] cols = new[] 
            { 
                new FileDataColumn { ColumnName = "First", ColumnType = typeof(double) } 
            };

            IDataReader dataReader = new FileDataReader(s, cols, '\n', ';', Encoding.Unicode, null);

            bool hasRecords = dataReader.Read();
            double number = (double)dataReader[0];
                    Assert.Equals(number, 10.0008);
        }


                [TestMethod]
        public void BoolShouldBeSupported()
        {
            Stream s = new MemoryStream(1000);
            for (int x = 0; x < 1; x++)
            {
                AddRecordToStream(s, "true");
            }
            s.Position = 0;
            FileDataColumn[] cols = new[] 
            { 
                new FileDataColumn { ColumnName = "First", ColumnType = typeof(bool) } 
            };

            IDataReader dataReader = new FileDataReader(s, cols, '\n', ';', Encoding.Unicode, null);

            bool hasRecords = dataReader.Read();
            bool boolValue = (bool)dataReader[0];
            Assert.IsTrue(boolValue);
        }

                [TestMethod]
        public void RecordManipulatorShouldSuccessFullyManipulate()
        {
            Stream s = new MemoryStream(1000);
            for (int x = 0; x < 10; x++)
            {
                AddRecordToStream(s, string.Format("{0}\n", (x * 10)));
            }
            s.Position = 0;
            FileDataColumn[] cols = new[] 
{ 
    new FileDataColumn { ColumnName = "First", ColumnType = typeof(int) } 
};

            IDataReader dataReader = new FileDataReader(
                s,
                cols,
                '\n',
                ';',
                Encoding.Unicode,
                record =>
                {

                    int currentValue = record.GetInt32(0);
                    record.SetValue(0, currentValue * 2);
                });

            for (int x = 0; x < 10; x++)
            {
                dataReader.Read();
                Assert.Equals(dataReader[0], x * 10 * 2);

            }
        }

        private void AddRecordToStream(Stream toStream, string record)
        {

            byte[] streamContents = Encoding.Unicode.GetBytes(record);
            toStream.Write(streamContents, 0, streamContents.Length);
        }

    }
}
