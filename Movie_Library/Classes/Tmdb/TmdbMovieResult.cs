using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Movie_Library.Classes.Tmdb
{
    /// <summary>
    /// Représente un film retourné par l'API TMDB.
    ///
    /// Cette classe ne représente pas directement un film
    /// de notre bibliothèque. Elle sert uniquement à récupérer
    /// et convertir les données JSON envoyées par TMDB.
    /// </summary>
    internal class TmdbMovieResult
    {
        /// <summary>
        /// Identifiant unique TMDB.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }


        /// <summary>
        /// Titre du film.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;


        /// <summary>
        /// Résumé du film.
        /// </summary>
        [JsonPropertyName("overview")]
        public string Overview { get; set; } = string.Empty;


        /// <summary>
        /// Date de sortie.
        /// </summary>
        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;


        /// <summary>
        /// Chemin de l'affiche.
        /// </summary>
        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }
    }
}
