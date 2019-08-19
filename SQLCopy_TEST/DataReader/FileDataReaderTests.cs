using System;
using System.Data;
using System.IO;
using System.Text;
using FileDataReader;
using NUnit.Framework;

namespace FileDataReaderTests
{
    [TestFixture]
    public class FileDataReaderTests
    {

        [Test]
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

            IDataReader dataReader = new FileDataReader.FileDataReader(s, cols, '\n', ',', Encoding.Unicode, null);

            bool hasRecords = dataReader.Read();
            Assert.That(hasRecords, Is.True);

        }

        [Test]
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

            IDataReader dataReader = new FileDataReader.FileDataReader(s, cols, '\n', ',', Encoding.Unicode, null);

            bool hasRecords = dataReader.Read();
            int first = (int)dataReader[0];
            Assert.That(first, Is.EqualTo(10));
            DateTime second = (DateTime)dataReader[1];
            Assert.That(second, Is.EqualTo(new DateTime(2010, 5, 5, 10, 0, 0)));

        }

        [Test]
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

            IDataReader dataReader = new FileDataReader.FileDataReader(s, cols, '\n', ',', Encoding.Unicode, null);

            dataReader.Read();
            DateTime dateTime = (DateTime)dataReader[0];
            Assert.That(dateTime, Is.EqualTo(new DateTime(2010, 5, 5, 10, 0, 1, 5)));
        }

        [Test]
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

            IDataReader dataReader = new FileDataReader.FileDataReader(s, cols, '\n', ';', Encoding.Unicode, null);

            bool hasRecords = dataReader.Read();
            double number = (double)dataReader[0];
            Assert.That(number, Is.EqualTo(10.0008));
        }


        [Test]
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

            IDataReader dataReader = new FileDataReader.FileDataReader(s, cols, '\n', ';', Encoding.Unicode, null);

            bool hasRecords = dataReader.Read();
            bool boolValue = (bool)dataReader[0];
            Assert.That(boolValue, Is.True);
        }

        [Test]
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

            IDataReader dataReader = new FileDataReader.FileDataReader(
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
                Assert.That(dataReader[0], Is.EqualTo(x * 10 * 2), x.ToString());

            }
        }

        private void AddRecordToStream(Stream toStream, string record)
        {

            byte[] streamContents = Encoding.Unicode.GetBytes(record);
            toStream.Write(streamContents, 0, streamContents.Length);
        }

    }
}
