using Aspose.Pdf;
using Aspose.Pdf.Text;
using OnlineJournal.Model;
using System;
using System.Data;

namespace OnlineJournal.Processings
{
    public class PDFFroming
    {
        public void FormAcademicRecord(string destinationFilePath, ref DataTable data, string courseName, CourseLecturers courseLecturers)
        {
            Document pdfDocument = new Document();
            Page page = pdfDocument.Pages.Add();

            TextFragment title = new TextFragment("ВІДОМІСТЬ ОБЛІКУ УСПІШНОСТІ №____________");
            title.TextState.FontSize = 16;
            title.HorizontalAlignment = HorizontalAlignment.Center;
            page.Paragraphs.Add(title);

            TextFragment date = new TextFragment("Дата заповнення відомості: " + DateTime.Now.ToShortDateString());
            date.TextState.FontSize = 14;
            date.HorizontalAlignment = HorizontalAlignment.Center;
            page.Paragraphs.Add(date);

            TextFragment subject = new TextFragment("Назва дисципліни: " + courseName);
            subject.TextState.FontSize = 14;
            page.Paragraphs.Add(subject);

            TextFragment controlForm = new TextFragment("Форма семестрового контролю: __________");
            controlForm.TextState.FontSize = 12;
            page.Paragraphs.Add(controlForm);

            TextFragment lecturer = new TextFragment("Лектор: " + courseLecturers.Responsible);
            lecturer.TextState.FontSize = 12;
            page.Paragraphs.Add(lecturer);

            TextFragment teachers = new TextFragment("Викладачі: " + courseLecturers.Responsible + ", " + courseLecturers.GetHelpersAsString());
            teachers.TextState.FontSize = 12;
            page.Paragraphs.Add(teachers);

            page.Paragraphs.Add(new HtmlFragment("<br>"));

            Table table = new Table();
            table.ColumnWidths = "300 200";
            table.Border = new BorderInfo(BorderSide.All, 0.5f);
            table.DefaultCellBorder = new BorderInfo(BorderSide.All, 0.5f);

            table.ImportDataTable(data, true, 0, 0);

            page.Paragraphs.Add(table);

            page.Paragraphs.Add(new HtmlFragment("<br><br>"));
            TextFragment signature = new TextFragment("Підпис: _______________");
            signature.TextState.FontSize = 14;
            page.Paragraphs.Add(signature);

            pdfDocument.Save(destinationFilePath);
        }
    }
}
