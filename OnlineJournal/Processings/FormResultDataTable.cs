using System;
using System.Data;
using System.Linq;

namespace OnlineJournal.Processings
{
    public class FormResultDataTable
    {
        public DataTable FormResult(ref DataTable data, bool isSetoff)
        {
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("Ім'я Прізвище По батькові");
            resultTable.Columns.Add("Результат");

            foreach (DataRow row in data.Rows)
            {
                int grade = row.ItemArray.Skip(1).Sum(mark => mark is DBNull ? 0 : (int)mark);
                string result = isSetoff ? ResultAsOffset(grade) : Result(grade);
                resultTable.Rows.Add(row.ItemArray[0], result);
            }

            return resultTable;
        }

        private string Result(int grade)
        {
            if (grade < 60)
                return "Незадовільно";

            if (grade < 75)
                return "Задовільно";

            if (grade < 90)
                return "Добре";

            return "Відмінно";
        }

        private string ResultAsOffset(int grade)
        {
            if (grade < 60)
                return "Не зараховано";

            return "Зараховано";
        }
    }
}
