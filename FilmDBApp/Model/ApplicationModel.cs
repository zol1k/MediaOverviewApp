
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace FilmDBApp.Model
{
    class ApplicationModel : ObservableObject
    {
        #region Fields
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ApplicationConfiguration _config;


        #endregion

        #region Properties / Commands

        public CollectionOfGenres CollectionOfGenres { get; set; }

        public ObservableCollection<Genre> ListOfGenres
        {
            get => CollectionOfGenres.GenreList;
        }

        public ApplicationConfiguration Config { get => _config; }

        #endregion

        public ApplicationModel()
        {
            _config = new ApplicationConfiguration();
            FillGenreCollectionByConfigurationFile();
        }

        public void FillGenreCollectionByConfigurationFile()
        {
            CollectionOfGenres = new CollectionOfGenres();
            List<string> genrePaths = _config.GenrePaths;
            CollectionOfGenres.ClearAll();

            foreach (string path in genrePaths)
            {
                CollectionOfGenres.AddNewGenre(new Genre(new FileInfo(path)));
            }
        }


        /// <summary>
        /// Going throught paths of recieved genres, and fill its filmLists with CollectGenreFilms
        /// </summary>
        /// <param name="CollectionOfGenres">collection of Genres</param>
        public void CollectGenreFilms()
        {
            List<string> errorList = new List<string>();

            foreach (Genre genre in ListOfGenres)
            {
                try
                {
                    CollectGenreFilms(genre);
                }
                catch (DirectoryNotFoundException msg)
                {
                    errorList.Add(genre.GenreName + " - " + genre.PathToGenreDirectory);
                    Log.Error(msg.ToString());
                }
            }
            if (errorList.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Bellow path to genre(s) Not Found.");
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(String.Join(Environment.NewLine, errorList.ToArray()));
                sb.AppendLine();
                sb.AppendLine();
                sb.Append("Please go to settings and choose your genre folder again !");
                MessageBox.Show(sb.ToString(), "Genres Not Found");
            }
        }

        private void CollectGenreFilms(Genre genre)
        {
            foreach (var file in Directory.GetFiles(genre.PathToGenreDirectory))
            {
                FileInfo fileInfo = new FileInfo(file);

                //if current file is not hidden, add it into film db
                if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    genre.CollectionOfFilms.AddNewFilm(new Film(fileInfo, false)
                    {
                        DirectoryGenre = genre.GenreName
                    });
                }
            }

            foreach (var file in Directory.GetDirectories(genre.PathToGenreDirectory))
            {
                FileInfo fileInfo = new FileInfo(file);

                if (CollectionOfGenres.GenreNameList.Contains(fileInfo.Name))
                    continue;
                //if current directory is not hidden, add it into film db
                if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    genre.CollectionOfFilms.AddNewFilm(new Film(fileInfo, true)
                    {
                        DirectoryGenre = genre.GenreName
                    });
                }
            }

        }
    }
}
