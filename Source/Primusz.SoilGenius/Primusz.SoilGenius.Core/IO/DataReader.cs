using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace Primusz.SoilGenius.Core.IO
{
    public class DataReader : IDisposable
    {
        #region Members

        private StreamReader reader;
        private DataTable dataTable;

        #endregion

        #region Properties

        public char ColumnSeparator { get; set; }

        public int RowCount => dataTable.Rows.Count;
        public int ColumnCount => dataTable.Columns.Count;

        #endregion

        #region Constructors

        public DataReader(Stream stream)
        {
            if (stream != null)
                reader = new StreamReader(stream);
            else
                throw new ArgumentNullException(nameof(stream));

            ColumnSeparator = '\t';
        }

        #endregion

        #region Methods

        public bool Read(string name = "Test", bool firstLineColumnNames = true)
        {
            dataTable = new DataTable(name);

            try
            {
                while (reader.EndOfStream == false)
                {
                    var line = reader.ReadLine();

                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] items = line.Trim().Split(ColumnSeparator);

                        if (dataTable.Columns.Count == 0)
                        {
                            // Create the data columns for the data table based on the number of items on the first line of the file
                            for (var i = 0; i < items.Length; i++)
                            {
                                var columnName = firstLineColumnNames ? items[i] : "Column" + i;
                                dataTable.Columns.Add(new DataColumn(columnName, typeof(string)));
                            }
                            if (firstLineColumnNames) continue;
                        }
                        dataTable.Rows.Add(items.ToArray<object>());
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                return false;
            }
        }

        public List<List<string>> GetRows()
        {
            return (from DataRow row in dataTable.Rows select row.ItemArray.Cast<string>().ToList()).ToList();
        }

        public List<string> GetRow(int index)
        {
            return dataTable.Rows[index].ItemArray.Cast<string>().ToList();
        }

        public List<string> GetColumn(int index)
        {
            return (from DataRow row in dataTable.Rows select (string)row[index]).ToList();
        }

        #endregion

        #region IDisposable Interface

        ~DataReader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }
            }
        }

        #endregion
    }
}