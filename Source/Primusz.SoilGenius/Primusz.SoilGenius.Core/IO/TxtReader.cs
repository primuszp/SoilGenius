using System;
using System.IO;

namespace Primusz.SoilGenius.Core.IO
{
    public class TxtReader : IDisposable
    {
        #region Members

        private StreamReader reader;

        #endregion

        #region Properties

        public char ColumnSeparator { get; set; }

        public char DecimalSeparator { get; set; }

        #endregion

        #region Constructors

        public TxtReader(Stream stream)
        {
            if (stream != null)
                reader = new StreamReader(stream);
            else
                throw new ArgumentNullException(nameof(stream));

            ColumnSeparator = '\t';
            DecimalSeparator = '.';
        }

        #endregion

        public void Read(bool firstLineColumnNames = true)
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (!string.IsNullOrEmpty(line))
                {

                }
            }
        }

        #region IDisposable Interface

        ~TxtReader()
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