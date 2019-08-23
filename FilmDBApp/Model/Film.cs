using System;
using System.IO;
using System.Text.RegularExpressions;

namespace WpfApp1
{
    public class Film : ObservableObject
    {
        #region Fields

        private string _fileExtension;
        private string _filePath;
        private string _filmNameCzsk;
        private string _filmNameEn;
        private string _filmYear;
        private FileInfo _filmFileInfo;
        private bool _isDirectory;

        #endregion

        public Film(FileInfo fileInfo, bool isDirectory = false)
        {
            this._filmFileInfo = fileInfo;
            this._isDirectory = isDirectory;
        }

        #region Properties

        public FileInfo FilmFileInfo
        {
            get => _filmFileInfo;
        }
        public string FilePath
        {
            get => FilmFileInfo.FullName;
        }
        public string FileName
        {
            get => Path.GetFileNameWithoutExtension(FilePath);
        }

        #endregion


        #region Methods

        private void ParseFileName(FileInfo fileInfo)
        {
            // if The following regex returns true as long as the string contains [] and () brackets
            if (Regex.IsMatch(FileName, @"\[.*?\]"))
                try
                {
                    _filmNameCzsk = Regex.Match(FileName, @"\[([^]]*)\]").Groups[1].Value;
                    _filmNameEn = Regex.Match(FileName, @"^.*?(?=\[)").Value;
                }
                catch (FormatException ex)
                {
                    _filmNameEn = FileName;
                }
            else
                _filmNameEn = Regex.Match(FileName, @"^.*?(?=\()").Value;

            if (Regex.IsMatch(FileName, @"\(.*?\)"))
                try
                {
                    _filmYear = Regex.Match(FileName, @"\(([^)]*)\)").Groups[1].Value;
                }
                catch (FormatException ex)
                {
                    _filmNameEn = FileName;
                }

            if (string.IsNullOrEmpty(_filmNameEn) && string.IsNullOrEmpty(_filmNameCzsk)) _filmNameEn = FileName;
        }


        #endregion
    }
}