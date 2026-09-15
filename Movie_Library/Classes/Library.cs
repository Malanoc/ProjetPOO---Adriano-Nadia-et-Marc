using Movie_Library.Classes.Tmdb;
using Movie_Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movie_Library.Classes
{
    /// <summary>
    /// Classe Library, qui représente une bibliothèque de films.
    /// </summary>
    internal class Library
    {
        /// <summary>
        /// Nom de la bibliothèque.
        /// </summary>
        private string _name;


        /// <summary>
        /// Liste des films actuellement présents
        /// dans la bibliothèque.
        /// </summary>
        private List<Movie> _movies;


        /// <summary>
        /// Liste des collections personnelles
        /// dans la bibliothèque.
        /// </summary>
        private List<Collection> _collections;


        /// <summary>
        /// Service permettant de communiquer avec TMDB.
        /// 
        /// Library ne connaît pas le fonctionnement de TMDB.
        /// Elle demande simplement à MoviesApi de rechercher
        /// des films.
        /// </summary>
        private readonly MoviesApi _moviesApi;


        /// <summary>
        /// Constructeur de la classe Library.
        /// </summary>
        /// <param name="name">
        /// Nom de la bibliothèque.
        /// </param>
        public Library(string name)
        {
            _name = name;

            /*
             * Initialisation de la liste des films.
             */
            _movies = new List<Movie>();


            /*
             * Initialisation de la liste des collections.
             */
            _collections = new List<Collection>();


            /*
             * Création du service chargé de communiquer
             * avec TMDB.
             */
            _moviesApi = new MoviesApi();
        }


        /// <summary>
        /// Retourne tous les films présents
        /// dans la bibliothèque.
        /// </summary>
        public List<Movie> Movies
        {
            get { return _movies; }
        }


        /// <summary>
        /// Retourne toutes les collections présentes
        /// dans la bibliothèque.
        /// </summary>
        public List<Collection> Collections
        {
            get { return _collections; }
        }


        /// <summary>
        /// Ajoute un film à la bibliothèque.
        /// </summary>
        /// <param name="movie">
        /// Film à ajouter.
        /// </param>
        public void addMovies(Movie movie)
        {
            /*
             * Vérifie si un film avec le même identifiant TMDB
             * est déjà présent dans la bibliothèque.
             */
            bool movieAlreadyExists =
                _movies.Any(existingMovie =>
                    existingMovie.Id == movie.Id
                );


            if (!movieAlreadyExists)
            {
                _movies.Add(movie);
            }
            else
            {
                Console.WriteLine(
                    $"Le film '{movie.Title}' est déjà présent dans la bibliothèque."
                );
            }
        }


        /// <summary>
        /// Supprime un film de la bibliothèque.
        /// </summary>
        /// <param name="movie">
        /// Film à supprimer.
        /// </param>
        public void removeMovie(Movie movie)
        {
            _movies.Remove(movie);
        }


        /// <summary>
        /// Recherche des films sur TMDB.
        ///
        /// Les résultats sont séparés en deux listes :
        ///
        /// - FoundMovies :
        ///   films qui ne sont pas encore
        ///   dans la bibliothèque.
        ///
        /// - AlreadyInLibrary :
        ///   films déjà présents dans la bibliothèque.
        /// </summary>
        /// <param name="title">
        /// Titre ou texte recherché.
        /// </param>
        /// <returns>
        /// Résultat contenant les films trouvés
        /// et les films déjà présents.
        /// </returns>
        public async Task<MovieSearchResult> searchMovie(
            string title)
        {
            /*
             * Création du résultat de recherche.
             */
            MovieSearchResult searchResult =
                new MovieSearchResult();


            /*
             * MoviesApi effectue toute la communication
             * avec TMDB.
             *
             * Library reçoit uniquement une liste de Movie.
             */
            List<Movie> moviesFound =
                await _moviesApi.SearchMoviesAsync(title);


            /*
             * Analyse de tous les films retournés par TMDB.
             */
            foreach (Movie movie in moviesFound)
            {
                /*
                 * Recherche si le film existe déjà
                 * dans notre bibliothèque.
                 *
                 * L'identifiant utilisé est directement
                 * l'identifiant TMDB.
                 */
                Movie? existingMovie =
                    _movies.FirstOrDefault(
                        libraryMovie =>
                            libraryMovie.Id == movie.Id
                    );


                /*
                 * Le film existe déjà.
                 *
                 * On retourne l'objet présent dans la
                 * bibliothèque afin de conserver notamment :
                 *
                 * - sa note personnelle ;
                 * - sa note utilisateur ;
                 * - son statut.
                 */
                if (existingMovie != null)
                {
                    searchResult
                        .AlreadyInLibrary
                        .Add(existingMovie);
                }


                /*
                 * Le film n'existe pas encore
                 * dans la bibliothèque.
                 */
                else
                {
                    searchResult
                        .FoundMovies
                        .Add(movie);
                }
            }


            return searchResult;
        }


        /// <summary>
        /// Trie les films de la bibliothèque
        /// par titre.
        /// </summary>
        /// <returns>
        /// Nouvelle liste de films triée.
        /// </returns>
        public List<Movie> sortMovie()
        {
            return _movies
                .OrderBy(movie => movie.Title)
                .ToList();
        }


        /// <summary>
        /// Ajoute une collection à la bibliothèque.
        /// </summary>
        /// <param name="collection">
        /// Collection à ajouter.
        /// </param>
        public void addCollection(
            Collection collection)
        {
            /*
             * Vérifie que la collection n'est pas
             * déjà présente.
             */
            if (!_collections.Contains(collection))
            {
                _collections.Add(collection);
            }
            else
            {
                Console.WriteLine(
                    "La collection est déjà présente dans la bibliothèque."
                );
            }
        }


        /// <summary>
        /// Supprime une collection de la bibliothèque.
        /// </summary>
        /// <param name="collection">
        /// Collection à supprimer.
        /// </param>
        public void removeCollection(
            Collection collection)
        {
            _collections.Remove(collection);
        }
    }
}