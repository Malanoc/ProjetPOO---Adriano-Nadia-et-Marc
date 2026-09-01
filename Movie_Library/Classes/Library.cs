using DotNetEnv;
using Movie_Library.Classes.Tmdb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
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
        /// HttpClient permet d'envoyer des requêtes HTTP.
        ///
        /// Nous l'utilisons pour communiquer avec l'API TMDB.
        ///
        /// Il est déclaré static afin de réutiliser la même instance
        /// pendant toute la durée de l'application.
        /// </summary>
        private static readonly HttpClient _httpClient = new HttpClient();


        /// <summary>
        /// Token permettant de s'authentifier auprès de TMDB.
        ///
        /// La valeur vient du fichier .env.
        /// </summary>
        private readonly string _tmdbApiToken;


        /// <summary>
        /// Constructeur de la classe Library.
        /// </summary>
        /// <param name="name">Nom de la bibliothèque.</param>
        public Library(string name)
        {
            _name = name;

            /*
             * Au moment de créer une bibliothèque,
             * sa liste de films est vide.
             */
            _movies = new List<Movie>();


            /*
             * Charge le fichier .env.
             *
             * TraversePath permet également de chercher le fichier
             * dans les dossiers parents si nécessaire.
             */
            Env.TraversePath().Load();


            /*
             * Récupère le token TMDB depuis le fichier .env.
             *
             * Si la variable n'existe pas, nous arrêtons le programme
             * avec une erreur explicite.
             */
            _tmdbApiToken =
                Environment.GetEnvironmentVariable("TMDB_API_TOKEN")
                ?? throw new InvalidOperationException(
                    "La variable TMDB_API_TOKEN est introuvable dans le fichier .env."
                );
        }


        /// <summary>
        /// Ajoute un film à la bibliothèque.
        /// </summary>
        /// <param name="movie">Film à ajouter.</param>
        public void addMovies(Movie movie)
        {
            /*
             * Vérifie d'abord qu'un film avec le même ID TMDB
             * n'est pas déjà présent.
             *
             * Cela permet d'éviter les doublons.
             */
            bool movieAlreadyExists =
                _movies.Any(existingMovie =>
                    existingMovie.TmdbId == movie.TmdbId
                );


            /*
             * On ajoute uniquement le film s'il n'existe pas déjà.
             */
            if (!movieAlreadyExists)
            {
                _movies.Add(movie);
            }
        }


        /// <summary>
        /// Supprime un film de la bibliothèque.
        /// </summary>
        /// <param name="movie">Film à supprimer.</param>
        public void removeMovie(Movie movie)
        {
            _movies.Remove(movie);
        }


        /// <summary>
        /// Recherche des films avec l'API TMDB.
        ///
        /// Tous les résultats trouvés sont ensuite comparés
        /// avec les films présents dans la bibliothèque.
        ///
        /// La méthode retourne deux listes :
        ///
        /// FoundMovies :
        /// films qui ne sont pas encore dans la bibliothèque.
        ///
        /// AlreadyInLibrary :
        /// films déjà présents dans la bibliothèque.
        /// </summary>
        /// <param name="title">Titre ou texte recherché.</param>
        /// <returns>
        /// Un objet MovieSearchResult contenant les deux listes.
        /// </returns>
        public async Task<MovieSearchResult> searchMovie(string title)
        {
            /*
             * Création de l'objet qui contiendra
             * les deux listes de résultats.
             */
            MovieSearchResult searchResult = new MovieSearchResult();


            /*
             * On vérifie que l'utilisateur a réellement
             * entré quelque chose.
             */
            if (string.IsNullOrWhiteSpace(title))
            {
                return searchResult;
            }


            /*
             * Uri.EscapeDataString transforme le texte afin
             * qu'il puisse être utilisé dans une URL.
             *
             * Exemple :
             *
             * Le Seigneur des Anneaux
             *
             * devient quelque chose comme :
             *
             * Le%20Seigneur%20des%20Anneaux
             */
            string encodedTitle =
                Uri.EscapeDataString(title);


            /*
             * Construction de l'URL permettant de rechercher
             * un film sur TMDB.
             *
             * query :
             * texte recherché.
             *
             * language=fr-FR :
             * demande les données traduites en français.
             *
             * include_adult=false :
             * exclut les contenus pour adultes.
             *
             * page=1 :
             * récupère la première page des résultats.
             */
            string url =
                $"https://api.themoviedb.org/3/search/movie" +
                $"?query={encodedTitle}" +
                $"&language=fr-FR" +
                $"&include_adult=false" +
                $"&page=1";


            /*
             * Création de la requête HTTP GET.
             */
            using HttpRequestMessage request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    url
                );


            /*
             * TMDB recommande d'utiliser le Read Access Token
             * comme Bearer Token dans l'en-tête Authorization.
             */
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _tmdbApiToken
                );


            /*
             * Nous indiquons que nous souhaitons recevoir
             * la réponse au format JSON.
             */
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(
                    "application/json"
                )
            );


            /*
             * Envoie la requête à TMDB.
             *
             * await attend la réponse sans bloquer
             * inutilement l'application.
             */
            using HttpResponseMessage response =
                await _httpClient.SendAsync(request);


            /*
             * Vérifie que la requête s'est correctement déroulée.
             *
             * Par exemple :
             *
             * 200 → OK
             * 401 → problème de token
             * 404 → ressource inexistante
             */
            response.EnsureSuccessStatusCode();


            /*
             * Récupère le JSON retourné par TMDB.
             */
            string json =
                await response.Content.ReadAsStringAsync();


            /*
             * Transforme le JSON en objet C#.
             */
            TmdbSearchResponse? tmdbResponse =
                JsonSerializer.Deserialize<TmdbSearchResponse>(
                    json
                );


            /*
             * Si l'API n'a retourné aucun résultat,
             * nous retournons simplement les deux listes vides.
             */
            if (tmdbResponse == null ||
                tmdbResponse.Results.Count == 0)
            {
                return searchResult;
            }


            /*
             * Nous parcourons maintenant TOUS les films
             * retournés par TMDB.
             */
            foreach (TmdbMovieResult tmdbMovie in tmdbResponse.Results)
            {
                /*
                 * Transformation du film TMDB
                 * en objet Movie utilisé par notre application.
                 */
                Movie movie = new Movie(
                    tmdbMovie.Id,
                    tmdbMovie.Title,
                    tmdbMovie.Overview,
                    tmdbMovie.ReleaseDate,
                    tmdbMovie.PosterPath
                );


                /*
                 * Recherche si ce film est déjà présent
                 * dans notre bibliothèque.
                 *
                 * Nous comparons les IDs TMDB.
                 *
                 * Exemple :
                 *
                 * The Batman (2022)
                 * TMDB ID : 414906
                 *
                 * Si cet ID existe déjà dans _movies,
                 * nous savons qu'il s'agit exactement
                 * du même film.
                 */
                Movie? existingMovie =
                    _movies.FirstOrDefault(
                        libraryMovie =>
                            libraryMovie.TmdbId ==
                            movie.TmdbId
                    );


                /*
                 * Si le film existe déjà dans la bibliothèque,
                 * nous l'ajoutons dans la liste
                 * AlreadyInLibrary.
                 */
                if (existingMovie != null)
                {
                    /*
                     * On retourne volontairement l'objet déjà
                     * présent dans notre bibliothèque.
                     *
                     * Ainsi, si notre Movie contient plus tard
                     * des informations supplémentaires locales,
                     * elles seront conservées.
                     */
                    searchResult.AlreadyInLibrary.Add(
                        existingMovie
                    );
                }
                else
                {
                    /*
                     * Sinon, le film est disponible sur TMDB
                     * mais n'existe pas encore dans la bibliothèque.
                     */
                    searchResult.FoundMovies.Add(
                        movie
                    );
                }
            }


            /*
             * Retourne finalement les deux listes.
             */
            return searchResult;
        }


        /// <summary>
        /// Trie les films de la bibliothèque par titre.
        /// </summary>
        /// <returns>
        /// Nouvelle liste triée par ordre alphabétique.
        /// </returns>
        public List<Movie> sortMovie()
        {
            return _movies
                .OrderBy(movie => movie.Title)
                .ToList();
        }
    }
}
