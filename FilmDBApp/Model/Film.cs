using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using FilmDBApp.Model;
using Newtonsoft.Json;

namespace FilmDBApp
{
    public class Film : ObservableObject, IComparable
    {
        #region Fields


        private string _filmNameCzsk;
        private string _filmNameEn;
        private string _filmYear;
        private string _fileName;
        private readonly FileInfo _filmFileInfo;
        public readonly bool IsDirectory;
        public string DirectoryGenre { get; set; }
        private WebClient webClient;

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
            get
            {
                if (IsDirectory)
                    return "";
                else
                    return Path.GetExtension(FilePath);
            }
        }
        public string FileSize
        {
            get
            {
                try
                {
                    if (IsDirectory == true)
                        return ActionSet.GetFolderSize(FilmFileInfo.FullName);
                    else
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
            this.IsDirectory = isDirectory;

            ParseFileName();
            webClient = new WebClient();
        }




        #region Methods

        private void ParseFileName()
        {
            if (IsDirectory)
                FileName = FilmFileInfo.Name;
            else
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


            string newFilename = "";

            if (newFilmName != "")
                newFilename = newFilmName;

            if (newCzFilmName != "")
                newFilename = newFilename.Trim() + " [" + newCzFilmName.Trim() + "]";

            if (newYear.Trim() != "")
                newFilename = newFilename.Trim() + " (" + newYear + ")";

            string newfilePathName = DirectoryPath + "\\" + newFilename.Replace(':', '-').Trim() + FileExtension;

            FilmFileInfo.MoveTo(newfilePathName);

            ParseFileName();

            RetrieveImdbInfo();

        }

        /*
         * RetrieveImdbInfo()
         * - function that is building string out of collected data and retrieving json 
         *   out of omdb service. After that deserialize of retrieved json into ImdbEntity.
         *   Retrieved data are avaliable inside of ImdbEntity properties
         */
        public void RetrieveImdbInfo()
        {
            ImdbInfo = new ImdbEntity();
            string searchBy = FilmNameEn != "" ? FilmNameEn : FilmNameCzsk;

            string json = "";

            json = webClient.DownloadString(AppSettings.ImdbURL + searchBy + AppSettings.ImdbURLYear + FilmYear + AppSettings.ImdbURLApi);

            ImdbInfo = JsonConvert.DeserializeObject<ImdbEntity>(json);

        }

        public int CompareTo(object obj)
        {

            Film a = this;
            Film b = (Film)obj;
            return string.Compare(a.FileName, b.FileName);

        }


        #endregion
    }
}