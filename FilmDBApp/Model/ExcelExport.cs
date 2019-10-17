using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace FilmDBApp.Model
{
    class ExcelExport : IDisposable
    {
        #region Fields
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private CollectionOfGenres _collectionOfGenres;
        readonly ExcelPackage _excelFile;

        private readonly string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";
        static readonly List<string[]> headerRow = new List<string[]>()
        {
            new string[] { "Title", "Year", "Rated" , "Country" }
        };
        #endregion

        public ExcelExport(CollectionOfGenres collectionOfGenres)
        {
            _excelFile = new ExcelPackage();
            _collectionOfGenres = collectionOfGenres;
        }

        #region Methods

        /// <summary>
        /// Starts the ExcelExport() execution.
        /// </summary>
        internal void Run()
        {
            CreateExcelFile();
        }

        /// <summary>
        /// Execute import to excel with predefined values.
        /// </summary>
        private void CreateExcelFile()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "List of movies", // Default file name
                DefaultExt = ".xlsx", // Default file extension
                Filter = "Excel documents (.xlsx)|*.xlsx" // Filter files by extension
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                PopulateExcelFile();
                SaveExcelFile(filename);
                System.Windows.MessageBox.Show("Excel export done!");
            }
        }

        /// <summary>
        /// Use in CreateExcelFile() - Create sheets, retrieve IMDBInfo() and fill sheets with obtained data.
        /// </summary>
        private void PopulateExcelFile()
        {
            // Go through GenreList and create new excel sheets
            foreach (var genre in _collectionOfGenres.GenreList)
            {
                _excelFile.Workbook.Worksheets.Add(genre.GenreName);
                var worksheet = _excelFile.Workbook.Worksheets[genre.GenreName];
                worksheet.Cells[headerRange].Style.Font.Bold = true;
                worksheet.Cells[headerRange].Style.Font.Size = 14;
                worksheet.Cells[headerRange].LoadFromArrays(headerRow);

                List<object[]> cellData = new List<object[]>();
                foreach (var film in genre.ListOfFilms)
                {
                    film.RetrieveImdbInfo();
                    cellData.Add(new object[] { film.FileName, film.FilmYear, film.ImdbInfo.ImdbRating, film.ImdbInfo.Released });
                }

                worksheet.Cells[2, 1].LoadFromArrays(cellData);
            }
        }

        /// <summary>
        /// Use in CreateExcelFile() - save generated, populated excel file.
        /// </summary>
        /// <param name="filename"></param>
        private void SaveExcelFile(string filename)
        {
            _excelFile.SaveAs(new FileInfo(filename));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _excelFile.Dispose();
            _collectionOfGenres = null;
        }

        #endregion
    }
}
