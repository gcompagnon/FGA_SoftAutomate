using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileDataReader;
using NUnit.Framework;


namespace FileDataReaderTests
{
    [TestFixture]
    public class FileRecordReaderTests
    {

        [Test]
        public void ShouldBeAbleToReadFirstRecordOfAStream()
        {

            Stream s = new MemoryStream();

            string contents = "10, 12\r\n11, 14";
            byte[] streamContents = Encoding.Unicode.GetBytes(contents);
            s.Write(streamContents, 0, streamContents.Length);
            s.Position = 0;
            FileRecordReader reader = new FileRecordReader(s, '\n', Encoding.Unicode);

            string firstRecord = reader.ReadNextRecord();

            Assert.That(firstRecord, Is.Not.Null);
            Assert.That(firstRecord, Is.EqualTo("10, 12\r"));
        }

        [Test]
        public void ShouldBeAbleToReadLastRecordOfAStream()
        {

            Stream s = new MemoryStream();

            string contents = "10, 12\r\n11, 14";
            byte[] streamContents = Encoding.Unicode.GetBytes(contents);
            s.Write(streamContents, 0, streamContents.Length);
            s.Position = 0;
            FileRecordReader reader = new FileRecordReader(s, '\n', Encoding.Unicode);

            string firstRecord = reader.ReadNextRecord();
            Console.WriteLine(firstRecord);
            string secondRecord = reader.ReadNextRecord();

            Assert.That(secondRecord, Is.Not.Null);
            Assert.That(secondRecord, Is.EqualTo("11, 14"));
            Console.WriteLine(secondRecord);
        }

        [Test]
        public void ShouldBeAbleToReadAllRecordsOfAStream()
        {
            Stream s = new MemoryStream();

            int recordsToCreateInStream = 1000;
            for (int x = 0; x < recordsToCreateInStream; x++)
            {
                AddRecordToStream(s, string.Format("{0}, {0}\n", x));
            }

            s.Position = 0;
            FileRecordReader reader = new FileRecordReader(s, '\n', Encoding.Unicode);
            List<string> records = new List<string>(recordsToCreateInStream);
            string record = reader.ReadNextRecord();
            while (record != null)
            {
                records.Add(record);
                record = reader.ReadNextRecord();
            }

            Assert.That(records.Count, Is.EqualTo(recordsToCreateInStream));
            for (int x = 0; x < recordsToCreateInStream; x++)
            {
                Assert.That(records[x], Is.EqualTo(string.Format("{0}, {0}", x)));
            }

        }

        private void AddRecordToStream(Stream toStream, string record)
        {

            byte[] streamContents = Encoding.Unicode.GetBytes(record);
            toStream.Write(streamContents, 0, streamContents.Length);
        }

    }
}
