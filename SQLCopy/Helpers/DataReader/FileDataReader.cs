using System;
using System.Data;
using System.IO;
using System.Text;

namespace Helpers.DataReader
{
    /// <summary>
    /// An <see cref="IDataReader"/> implementation that provides forward only access
    /// to a stream where the format of the stream is defined in the constructor.
    /// from <see href="http://blog.smithfamily.dk"/>
    /// </summary>
    public class FileDataReader : FileDataRecord, IDataReader
    {
        /// <summary>
        /// The Reader instance used to read records from the stream.
        /// </summary>
        private readonly FileRecordReader fileRecordReader;

        /// <summary>
        /// The separator character for separating the different fields from each other
        /// </summary>
        private readonly char fieldSeparator;

        private readonly Action<FileDataRecord> recordManipulator;

        /// <summary>
        /// Controls whether or not the filestream has been closed.
        /// </summary>
        private bool isClosed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataReader"/> class.
        /// that uses \r\n as the record separator.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="columns">The columns describing the format of the stream for a single record.</param>
        /// <param name="fieldSeparator">The field separator.</param>
        /// <param name="fileEncoding">The file encoding.</param>
        public FileDataReader(Stream fileStream, FileDataColumn[] columns, char fieldSeparator, Encoding fileEncoding)
            : this(fileStream, columns, '\n', fieldSeparator, fileEncoding, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataReader"/> class.
        /// This constructor will use the \n as the record separator and ; as the field separator
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="fileEncoding">The file encoding.</param>
        public FileDataReader(Stream fileStream, FileDataColumn[] columns, Encoding fileEncoding)
            : this(fileStream, columns, '\n', ';', fileEncoding, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataReader"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="columns">The columns describing the format of the stream for a single record.</param>
        /// <param name="recordSeparator">The record separator.</param>
        /// <param name="fieldSeparator">The field separator.</param>
        /// <param name="fileEncoding">The file encoding.</param>
        /// <param name="recordManipulator">The record manipulator.</param>
        public FileDataReader(Stream fileStream, FileDataColumn[] columns, char recordSeparator, char fieldSeparator, Encoding fileEncoding, Action<FileDataRecord> recordManipulator)
            : base(columns)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException("fileStream");
            }

            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }

            if (columns.Length == 0)
            {
                throw new ArgumentOutOfRangeException("columns", "the length of the column array must be greater than 0");
            }

            this.fileRecordReader = new FileRecordReader(fileStream, recordSeparator, fileEncoding);
            this.fieldSeparator = fieldSeparator;
            this.recordManipulator = recordManipulator;
        }

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        /// <value></value>
        /// <returns>The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for SELECT statements.</returns>
        public int RecordsAffected
        {
            get
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        /// <value></value>
        /// <returns>The level of nesting.</returns>
        public int Depth
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        /// <value></value>
        /// <returns>true if the data reader is closed; otherwise, false.</returns>
        public bool IsClosed
        {
            get
            {
                return isClosed;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Nothing to do here, we should not dispose of the filestream passed in. 
            // The calling class should do that.
            if (!isClosed)
            {
                Close();
            }
        }

        /// <summary>
        /// Closes the <see cref="T:System.Data.IDataReader"/> Object.
        /// </summary>
        public void Close()
        {
            isClosed = true;
            fileRecordReader.Close();
        }

        /// <summary>
        /// Returns a <see cref="T:System.Data.DataTable"/> that describes the column metadata of the <see cref="T:System.Data.IDataReader"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.DataTable"/> that describes the column metadata.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.IDataReader"/> is closed. </exception>
        public DataTable GetSchemaTable()
        {
            DataTable table = new DataTable();

            foreach (FileDataColumn column in Columns)
            {
                table.Columns.Add(column.ColumnName, column.ColumnType);
            }

            return table;
        }

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        public bool NextResult()
        {
            // Only one result set, the current.
            return false;
        }

        /// <summary>
        /// Advances the <see cref="T:System.Data.IDataReader"/> to the next record.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        public bool Read()
        {
            // Try to read out a record from the stream
            string record = fileRecordReader.ReadNextRecord();
            if (record == null)
            {
                return false;
            }

            // Split the record in its column parts
            string[] recordParts = record.Split(new[] { fieldSeparator }, StringSplitOptions.None);
            if (recordParts.Length != Columns.Length)
            {
                throw new InvalidOperationException(string.Format("Not enough columns in line, expected:{0}, actual:{1}, line:{2}", Columns.Length, recordParts.Length, record));
            }

            // Iterate over each column and convert the record to its true type
            for (int x = 0; x < Columns.Length; x++)
            {
                object value = Convert.ChangeType(recordParts[x], Columns[x].ColumnType);
                SetValue(x, value);
            }
            
            if (recordManipulator != null)
            {
                recordManipulator(this);
            }

            return true;
        }
    }
}
