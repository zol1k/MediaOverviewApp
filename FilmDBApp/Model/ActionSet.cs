using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    }
}
