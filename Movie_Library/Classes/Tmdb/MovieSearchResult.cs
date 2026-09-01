using System.Collections.Generic;

namespace Movie_Library.Classes.Tmdb
{
    /// <summary>
    /// Représente le résultat d'une recherche de films.
    ///
    /// La recherche sépare les films en deux catégories :
    ///
    /// - FoundMovies :
    ///   films trouvés sur TMDB qui ne sont pas encore
    ///   présents dans la bibliothèque.
    ///
    /// - AlreadyInLibrary :
    ///   films trouvés par TMDB mais qui sont déjà
    ///   présents dans la bibliothèque.
    /// </summary>
    internal class MovieSearchResult
    {
        /// <summary>
        /// Films trouvés qui peuvent être ajoutés
        /// à la bibliothèque.
        /// </summary>
        public List<Movie> FoundMovies { get; set; } = new();


        /// <summary>
        /// Films trouvés qui sont déjà présents
        /// dans la bibliothèque.
        /// </summary>
        public List<Movie> AlreadyInLibrary { get; set; } = new();
    }
}
