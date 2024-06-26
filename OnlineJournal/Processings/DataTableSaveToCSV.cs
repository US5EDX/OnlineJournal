using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineJournal.Processings
{
    public class DataTableSaveToCSV
    {
        public bool Save(string destinationFilePath, ref DataTable dataTable)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                sb.AppendLine(string.Join(";", columnNames));

                foreach (DataRow row in dataTable.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    sb.AppendLine(string.Join(";", fields));
                }

                File.WriteAllText(destinationFilePath, sb.ToString(), Encoding.UTF8);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
