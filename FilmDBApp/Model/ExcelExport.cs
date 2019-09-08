﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace FilmDBApp.Model
{
    class ExcelExport:IDisposable
    {
        private CollectionOfGenres _collectionOfGenres;
        ExcelPackage _excelFile;
        string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";

        static List<string[]> headerRow = new List<string[]>()
        {
            new string[] { "Title", "Year", "Rated" , "Country" }
        };

        public ExcelExport(CollectionOfGenres collectionOfGenres)
        {
            _excelFile = new ExcelPackage();
            _collectionOfGenres = collectionOfGenres;
            CreateExcelFile();
        }

        private void CreateExcelFile()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "List of movies"; // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Excel documents (.xlsx)|*.xlsx"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // SaveExcelFile document
                string filename = dlg.FileName;
                PopulateExcelFile();
                SaveExcelFile(filename);
            }
        }

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

        private async Task PopulateExcelFileAsync()
        {


        }

        private void SaveExcelFile(string filename)
        {
            _excelFile.SaveAs(new FileInfo(filename));
        }

        public void Dispose()
        {
            _excelFile = null;
            _collectionOfGenres = null;

        }
    }
}