using FBlock.Core;
using System;
using System.Data;
using System.IO;

namespace FBlock.Components.IO
{
    /// <summary>
    /// Read a CSV at the specified location and returns a DataSet
    /// </summary>
    public class CSVReader : Component<string, DataTable>
    {
        protected string[] _separator;
        protected bool _hasHeader;

        public CSVReader(bool hasHeader = false, string separator = ",")
        {
            _separator = new string[] { separator };
            _hasHeader = hasHeader;
        }

        public override DataTable Process(string filepath, JobContext context)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException();

            DataTable dt = new DataTable();

            using (var sr = new StreamReader(filepath))
            {
                // Try to retrieve the number of columns
                int nbColumns = 0;

                try
                {
                    string[] firstLine = sr.ReadLine().Split(_separator, StringSplitOptions.None);

                    nbColumns = firstLine.Length;

                    foreach (string head in firstLine)
                    {
                        if (_hasHeader)
                            dt.Columns.Add(head);
                        else
                            dt.Columns.Add();
                    }

                    // If it has no header, don't consume the first line
                    if (!_hasHeader)
                    {
                        sr.BaseStream.Position = 0;
                        sr.DiscardBufferedData();
                    }
                }
                catch { }

                if (nbColumns != 0)
                {
                    while (!sr.EndOfStream)
                    {
                        string[] row = sr.ReadLine().Split(_separator, StringSplitOptions.None);

                        DataRow dr = dt.NewRow();

                        for (int i = 0; i < nbColumns; ++i)
                            dr[i] = row[i];

                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }
    }
}
