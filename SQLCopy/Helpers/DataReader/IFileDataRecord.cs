using System.Data;

namespace Helpers.DataReader
{
    /// <summary>
    /// Represents a record that is contained in a IFileDataReader
    /// </summary>
    public interface IFileDataRecord : IDataRecord
    {
        /// <summary>
        /// Sets the values on the given record.
        /// The input array's length must match the number of columns in the record.
        /// It is okay to pass null into the columns.
        /// </summary>
        /// <param name="values">The values to set</param>
        void SetValues(object[] values);

        /// <summary>
        /// Sets the value at the given index in the row.
        /// </summary>
        /// <param name="index">The index of the column to set the value at</param>
        /// <param name="value">The value to set.</param>
        void SetValue(int index, object value);
    }
}
