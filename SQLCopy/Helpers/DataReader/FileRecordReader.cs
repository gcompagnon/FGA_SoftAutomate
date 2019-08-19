using System.IO;
using System.Text;

namespace Helpers.DataReader
{
    /// <summary>
    /// Class that can read from a stream and return a part of the string based on a
    /// record separator.
    /// Works like StreamReaders ReadLine except that ReadLine is hardcoded to \r\n and this class
    /// can use any character as the separator.
    /// </summary>
    public class FileRecordReader
    {
        /// <summary>
        /// The underlying stream where this reader reads the records from
        /// </summary>
        private readonly Stream fileStream;

        /// <summary>
        /// The character that separates the records.
        /// </summary>
        private readonly char recordSeparator;

        /// <summary>
        /// The encoding to use when decoding the byte stream into characters
        /// </summary>
        private readonly Encoding fileEncoding;

        /// <summary>
        /// The char buffer where the bytes read from the stream will be stored until they have to be used.
        /// </summary>
        private readonly char[] stringBuffer = new char[1024];

        /// <summary>
        /// The current position within the char buffer
        /// </summary>
        private int charPos;

        /// <summary>
        /// The number of characters the current char buffer contains.
        /// </summary>
        private int charsRead;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRecordReader"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="recordSeparator">The record separator.</param>
        /// <param name="fileEncoding">The file encoding.</param>
        public FileRecordReader(Stream fileStream, char recordSeparator, Encoding fileEncoding)
        {
            this.fileStream = fileStream;
            this.recordSeparator = recordSeparator;
            this.fileEncoding = fileEncoding;
        }

        /// <summary>
        /// Closes this instance and the underlying stream.
        /// </summary>
        public void Close()
        {
            fileStream.Close();
        }

        /// <summary>
        /// Reads the next record from the stream.
        /// </summary>
        /// <returns>The next record from the stream or null if no more records exist.</returns>
        public string ReadNextRecord()
        {
            StringBuilder builder = null;

            // Check if we are at the end of the stream
            if ((charPos == charsRead) && (ReadIntoBuffer() == 0))
            {
                return null;
            }

            // Outer loop that will loop while we are capable of reading stuff into the char buffer
            do
            {
                // Set the current inner position to be equal to the position in the char buffer
                int innerCharPos = charPos;

                // Inner loop that will loop until there are no more characters in the buffer, or we have reached end of record.
                do
                {
                    // Is current char the record separator, then either add to string buffer and return or just return the part of the buffer up 
                    if (stringBuffer[innerCharPos] == recordSeparator)
                    {
                        string stringToReturn;
                        if (builder != null)
                        {
                            // this is at least the second time we read a full buffer, add the buffer up until charpos into the builder and lets return
                            builder.Append(stringBuffer, charPos, innerCharPos - charPos);
                            stringToReturn = builder.ToString();
                        }
                        else
                        {
                            // This if first pass of the char buffer, so just return that part of the char buffer
                            stringToReturn = new string(stringBuffer, charPos, innerCharPos - charPos);
                        }

                        charPos = innerCharPos + 1;
                        return stringToReturn;
                    }

                    innerCharPos++;
                }
                while (innerCharPos < charsRead);

                innerCharPos = charsRead - charPos;
                if (builder == null)
                {
                    builder = new StringBuilder(1024);
                }

                builder.Append(stringBuffer, charPos, innerCharPos);
            }
            while (ReadIntoBuffer() > 0);
            return builder.ToString();
        }

        /// <summary>
        /// Read at most 1024 bytes into the character buffer, using whatever encoding that was passed.
        /// </summary>
        /// <returns>the number of characters read into the buffer</returns>
        private int ReadIntoBuffer()
        {
            byte[] byteBuffer = new byte[1024];
            charPos = 0;
            charsRead = 0;
            int bytesRead = fileStream.Read(byteBuffer, 0, byteBuffer.Length);

            if (bytesRead == 0)
            {
                return 0;
            }

            charsRead = fileEncoding.GetChars(byteBuffer, 0, bytesRead, stringBuffer, 0);
            return charsRead;
        }
    }
}
