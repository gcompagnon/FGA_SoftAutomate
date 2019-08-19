using System;

namespace Helpers.DataReader
{
    /// <summary>
    /// Represents a column in a file. 
    /// </summary>
    public class FileDataColumn : IComparable<FileDataColumn>
    {
        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the column.
        /// </summary>
        /// <value>The type of the column.</value>
        public Type ColumnType
        {
            get;
            set;
        }
        
        /// <summary>
        /// Compares the current FileDataRecord with another FileDataRecord.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: 
        /// Less than zero - This object is less than the other parameter.
        /// Zero - This object is equal to other. 
        /// Greater than zero - This object is greater than other. </returns>
        public int CompareTo(FileDataColumn other)
        {
            return ColumnName.CompareTo(other.ColumnName);
        }
    }
}
