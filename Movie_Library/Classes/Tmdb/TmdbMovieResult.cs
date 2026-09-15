using System.Text.Json.Serialization;

namespace Movie_Library.Classes.Tmdb
{
    /// <summary>
    /// Représente un film retourné par l'API TMDB.
    ///
    /// Cette classe sert uniquement à convertir
    /// les informations JSON de TMDB.
    /// </summary>
    internal class TmdbMovieResult
    {
        /// <summary>
        /// Identifiant unique du film sur TMDB.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }


        /// <summary>
        /// Titre du film.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;


        /// <summary>
        /// Synopsis du film.
        /// </summary>
        [JsonPropertyName("overview")]
        public string Overview { get; set; } = string.Empty;


        /// <summary>
        /// Chemin vers le poster du film.
        /// </summary>
        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }


        /// <summary>
        /// Score du film provenant de TMDB.
        /// </summary>
        [JsonPropertyName("vote_average")]
        public float VoteAverage { get; set; }
    }
}