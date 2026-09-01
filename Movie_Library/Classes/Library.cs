using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie_Library.Classes
{
    /// <summary>
    /// Classe Library, qui représente une bibliothèque de films.
    /// </summary>
    internal class Library
    {
        private string _name;
        private List<Movie> _movies;

        /// <summary>
        /// Constructeur de la classe Library.
        /// </summary>
        /// <param name="name">Le nom de la bibliothèque.</param>
        public Library(string name)
        {
            _name = name;
            _movies = new List<Movie>();
        }

        /// <summary>
        /// Méthode pour ajouter des films à la bibliothèque.
        /// </summary>
        /// <param name="movie">Le film à ajouter.</param>
        public void addMovies(Movie movie)
        {
            // Implementation for adding movies to the library
        }

        /// <summary>
        /// Méthode pour supprimer des films de la bibliothèque.
        /// </summary>
        /// <param name="movie">Le film à supprimer.</param>
        public void removeMovie(Movie movie)
        {
            // Implementation for removing movies from the library
        }

        /// <summary>
        /// Méthode pour trier les films dans la bibliothèque.
        /// </summary>
        /// <param name="movie">Le film à trier.</param>
        /// <returns>La liste des films triés.</returns>
        public List<Movie> sortMovie()
        {
            // Implementation for sorting movies in the library
        }

        /// <summary>
        /// Méthode pour rechercher un film dans la bibliothèque.
        /// </summary>
        /// <param name="title">Le titre du film à rechercher.</param>
        /// <returns>Le film trouvé, ou null s'il n'est pas trouvé.</returns>
        public Movie searchMovie(string title)
        {
            // Implementation for searching movies in the library
        }
    }
}
