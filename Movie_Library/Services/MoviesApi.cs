using DotNetEnv;
using Movie_Library.Classes;
using Movie_Library.Classes.Tmdb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Movie_Library.Services
{
    /// <summary>
    /// Service responsable de la communication avec l'API TMDB.
    /// 
    /// Cette classe s'occupe uniquement de :
    /// - charger le token TMDB ;
    /// - envoyer les requêtes HTTP ;
    /// - récupérer les réponses JSON ;
    /// - convertir les résultats TMDB en objets Movie.
    /// </summary>
    internal class MoviesApi
    {
        /// <summary>
        /// Adresse de base de l'API TMDB.
        /// </summary>
        private const string _baseUrl =
            "https://api.themoviedb.org/3";


        /// <summary>
        /// Client HTTP utilisé pour communiquer avec TMDB.
        /// 
        /// Il est static afin de réutiliser la même instance
        /// pendant toute la durée de l'application.
        /// </summary>
        private static readonly HttpClient _httpClient =
            new HttpClient();


        /// <summary>
        /// Token permettant de s'authentifier auprès de TMDB.
        /// </summary>
        private readonly string _tmdbApiToken;


        /// <summary>
        /// Constructeur du service MoviesApi.
        /// </summary>
        public MoviesApi()
        {
            /*
             * Charge le fichier .env.
             */
            Env.TraversePath().Load();


            /*
             * Récupère le token TMDB depuis le fichier .env.
             */
            _tmdbApiToken =
                Environment.GetEnvironmentVariable(
                    "TMDB_API_TOKEN"
                )
                ?? throw new InvalidOperationException(
                    "La variable TMDB_API_TOKEN est introuvable dans le fichier .env."
                );
        }


        /// <summary>
        /// Recherche des films sur TMDB à partir
        /// d'un titre ou d'une partie de titre.
        /// </summary>
        /// <param name="title">
        /// Titre ou texte à rechercher.
        /// </param>
        /// <returns>
        /// Liste des films retournés par TMDB.
        /// </returns>
        public async Task<List<Movie>> SearchMoviesAsync(
            string title)
        {
            /*
             * Si aucun texte n'a été saisi,
             * on retourne directement une liste vide.
             */
            if (string.IsNullOrWhiteSpace(title))
            {
                return new List<Movie>();
            }


            /*
             * Prépare le texte pour pouvoir l'utiliser
             * correctement dans une URL.
             *
             * Exemple :
             *
             * Star Wars
             *
             * devient :
             *
             * Star%20Wars
             */
            string encodedTitle =
                Uri.EscapeDataString(title);


            /*
             * Construction de l'URL de recherche TMDB.
             *
             * language=fr-FR :
             * demande les informations en français.
             *
             * include_adult=false :
             * exclut les contenus adultes.
             *
             * page=1 :
             * récupère la première page des résultats.
             */
            string url =
                $"{_baseUrl}/search/movie" +
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
             * Ajoute le token TMDB dans l'en-tête
             * Authorization de la requête.
             */
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _tmdbApiToken
                );


            /*
             * Indique que nous souhaitons recevoir
             * une réponse JSON.
             */
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(
                    "application/json"
                )
            );


            /*
             * Envoi de la requête vers TMDB.
             */
            using HttpResponseMessage response =
                await _httpClient.SendAsync(request);


            /*
             * Vérifie que TMDB a répondu correctement.
             *
             * Une exception sera générée en cas
             * d'erreur HTTP.
             */
            response.EnsureSuccessStatusCode();


            /*
             * Récupère la réponse JSON sous forme de texte.
             */
            string json =
                await response.Content.ReadAsStringAsync();


            /*
             * Transforme la réponse JSON en
             * TmdbSearchResponse.
             */
            TmdbSearchResponse? tmdbResponse =
                JsonSerializer.Deserialize<TmdbSearchResponse>(
                    json
                );


            /*
             * Si TMDB n'a retourné aucune réponse exploitable,
             * on retourne une liste vide.
             */
            if (tmdbResponse == null)
            {
                return new List<Movie>();
            }


            /*
             * Transformation des TmdbMovieResult
             * en objets Movie utilisés dans l'application.
             *
             * Les informations personnelles :
             *
             * - PersonalRating
             * - PersonalNote
             *
             * seront initialisées automatiquement par
             * le constructeur de Movie.
             */
            List<Movie> movies =
                tmdbResponse.Results
                    .Select(tmdbMovie =>
                        new Movie(
                            tmdbMovie.Id,
                            tmdbMovie.Title,
                            tmdbMovie.Overview,
                            tmdbMovie.PosterPath ?? "",
                            tmdbMovie.VoteAverage,
                            Status.NotSeen
                        )
                    )
                    .ToList();


            return movies;
        }
    }
}