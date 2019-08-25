using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using FilmDBApp.Model;
using WpfApp1.Model;
using Newtonsoft.Json;

namespace WpfApp1
{
    public class Film : ObservableObject
    {
        #region Fields


        private string _filmNameCzsk;
        private string _filmNameEn;
        private string _filmYear;
        private string _fileName;
        private readonly FileInfo _filmFileInfo;
        private readonly bool _isDirectory;

        #endregion

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
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged("FileName");
            }

        }

        public string FileExtension
        {
            get => Path.GetExtension(FilePath);
        }
        public string FileSize
        {
            get
            {
                try
                {
                    return ActionSet.FormatSize(FilmFileInfo.Length);
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    return "";
                }
            }
        }

        public string FilmNameCzsk
        {
            get => _filmNameCzsk;
            set
            {
                _filmNameCzsk = value;
                OnPropertyChanged("FilmNameCzsk");
            }
        }

        public string FilmNameEn
        {
            get => _filmNameEn;
            set
            {
                _filmNameEn = value;
                OnPropertyChanged("FilmNameEn");
            }
        }

        public string FilmYear
        {
            get => _filmYear;
            set
            {
                _filmYear = value;
               OnPropertyChanged("FilmYear");
            }
        }

        private string DirectoryPath
        {
            get => Path.GetDirectoryName(FilePath);
        }

        public ImdbEntity ImdbInfo { get; set; }

        #endregion




        public Film(FileInfo fileInfo, bool isDirectory)
        {
            this._filmFileInfo = fileInfo;
            this._isDirectory = isDirectory;

            ParseFileName();

            ImdbInfo = new ImdbEntity();
        }




        #region Methods

        private void ParseFileName()
        {
            FileName = Path.GetFileNameWithoutExtension(FilePath);
            // if The following regex returns true as long as the string contains [] and () brackets
            if (Regex.IsMatch(FileName, @"\[.*?\]"))
            {
                try
                {
                    FilmNameCzsk = Regex.Match(FileName, @"\[([^]]*)\]").Groups[1].Value;
                    FilmNameEn = Regex.Match(FileName, @"^.*?(?=\[)").Value;
                }
                catch (FormatException ex)
                {
                    string error = ex.Message;
                    FilmNameEn = FileName;
                }
            }
            else
            {
                FilmNameEn = Regex.Match(FileName, @"^.*?(?=\()").Value;
            }

            if (Regex.IsMatch(FileName, @"\(.*?\)"))
            {
                try
                {
                    FilmYear = Regex.Match(FileName, @"\(([^)]*)\)").Groups[1].Value;
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    FilmNameEn = FileName;
                }
            }

            if (string.IsNullOrEmpty(FilmNameEn) && string.IsNullOrEmpty(FilmNameCzsk))
                FilmNameEn = FileName;
        }

        public void ChangeFileName(string newFilmName, string newCzFilmName, string newYear)
        {
            newFilmName = newFilmName ?? ""; // if null then ""
            newCzFilmName = newCzFilmName ?? "";
            newYear = newYear ?? "";


            var newFilename = "";

            if (newFilmName.Trim() != "")
                newFilename = newFilmName;

            if (newCzFilmName.Trim() != "")
                newFilename = newFilename + " [" + newCzFilmName + "]";

            if (newYear.Trim() != "")
                newFilename = newFilename + " (" + newYear + ")";

            var newfilePathName = DirectoryPath + "\\" + newFilename + FileExtension;

            FilmFileInfo.MoveTo(newfilePathName);

            ParseFileName();
        }

        /*
         * RetrieveImdbInfo()
         * - function that is building string out of collected data and retrieving json 
         *   out of omdb service. After that deserialize of retrieved json into ImdbEntity.
         *   Retrieved data are avaliable inside of ImdbEntity properties
         */
        public void RetrieveImdbInfo()
        {
            WebClient wc = new WebClient();

            string searchBy = FilmNameEn != "" ? FilmNameEn : FilmNameCzsk;

            string json = "";

            json = wc.DownloadString(AppSettings.ImdbURL + searchBy + AppSettings.ImdbURLYear + FilmYear + AppSettings.ImdbURLApi);

            ImdbInfo = JsonConvert.DeserializeObject<ImdbEntity>(json);
        }


        #endregion
    }
}