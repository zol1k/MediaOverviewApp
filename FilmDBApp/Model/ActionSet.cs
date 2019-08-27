using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1;
using WpfApp1.Model;

namespace FilmDBApp.Model
{
    static class ActionSet
    {

        public static void ChangeFilmGenre(Film filmToMove, Genre filmOldGenre, Genre filmNewGenre)
        {
            filmOldGenre.ListOfFilms.Remove(filmToMove);
            filmNewGenre.ListOfFilms.Add(filmToMove);
            string path = filmNewGenre.PathToGenreDirectory;
            filmToMove.FilmFileInfo.MoveTo(path + "\\" + filmToMove.FilmFileInfo.Name);
        }


        private static void CollectGenreFilms(Genre genre, List<string> listToIgnore)
        {
            try
            {

                foreach (var file in Directory.GetFiles(genre.PathToGenreDirectory))
                {
                    FileInfo fileInfo = new FileInfo(file);

                    //if current file is not hidden, add it into film db
                    if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                        genre.CollectionOfFilms.AddNewFilm(new Film(fileInfo, false));
                }

                foreach (var file in Directory.GetDirectories(genre.PathToGenreDirectory))
                {
                    FileInfo fileInfo = new FileInfo(file);

                    if (listToIgnore.Contains(fileInfo.Name))
                        continue;
                    //if current directory is not hidden, add it into film db
                    if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                        genre.CollectionOfFilms.AddNewFilm(new Film(fileInfo, true));
                }

                //genre.Films = genre.Films.OrderBy(o => o.FileName).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void CollectGenreFilms(ObservableCollection<Genre> CollectionOfGenres)
        {
            List<string> namesOfGenres = new List<string>();

            // Firstly get List<namesOfGenres> to not show genre folders inside of FilmList
            foreach (var genre in CollectionOfGenres)
            {
                namesOfGenres.Add(genre.GenreName);
            }

            //
            foreach (var genre in CollectionOfGenres)
            {
                CollectGenreFilms(genre, namesOfGenres);
            }
        }


        static readonly string[] suffixes =
            { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        public static string FormatSize(Int64 bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }
    }
}
