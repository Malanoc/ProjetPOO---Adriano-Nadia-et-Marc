using System.Collections.Generic;

namespace Movie_Library.Classes
{
    /// <summary>
    /// Représente une collection de films.
    /// 
    /// Une collection possède :
    /// - un nom ;
    /// - un poster ;
    /// - une liste de films.
    /// </summary>
    internal class Collection
    {
        // Nom de la collection.
        private string _name;

        // Chemin ou URL du poster représentant la collection.
        private string _poster;

        // Liste des films appartenant à la collection.
        private List<Movie> _movies;


        /// <summary>
        /// Constructeur de la classe Collection.
        /// </summary>
        /// <param name="name">Nom de la collection.</param>
        /// <param name="poster">Poster de la collection.</param>
        public Collection(string name, string poster)
        {
            _name = name;
            _poster = poster;

            // Une nouvelle liste est créée pour stocker
            // les films de cette collection.
            _movies = new List<Movie>();
        }


        /// <summary>
        /// Lecture et modification du nom de la collection.
        /// </summary>
        /// <returns>Nom de la collection.</returns>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        /// <summary>
        /// Lecture et modification du poster de la collection.
        /// </summary>
        /// <returns>Poster de la collection.</returns>
        public string Poster
        {
            get { return _poster; }
            set { _poster = value; }
        }


        /// <summary>
        /// Retourne tous les films de la collection.
        /// </summary>
        /// <returns>Liste des films.</returns>
        public List<Movie> Movies
        {
            get { return _movies; }
        }


        /// <summary>
        /// Ajoute un film à la collection.
        /// </summary>
        /// <param name="movie">Film à ajouter.</param>
        public void addMovie(Movie movie)
        {
            /*
             * On vérifie que le film n'est pas déjà présent
             * pour éviter les doublons dans la collection.
             */
            if (!_movies.Contains(movie))
            {
                _movies.Add(movie);
            }
            else
            { 
                Console.WriteLine($"Le film '{movie.Title}' est déjà présent dans la collection.");
            }
        }


        /// <summary>
        /// Supprime un film de la collection.
        /// </summary>
        /// <param name="movie">Film à supprimer.</param>
        public void RemoveMovie(Movie movie)
        {
            _movies.Remove(movie);
        }
    }
}