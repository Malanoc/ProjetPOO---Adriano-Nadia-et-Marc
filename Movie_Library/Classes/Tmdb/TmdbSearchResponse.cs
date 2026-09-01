using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Movie_Library.Classes.Tmdb
{
    /// <summary>
    /// Représente la réponse globale retournée par TMDB
    /// lors d'une recherche de films.
    /// </summary>
    internal class TmdbSearchResponse
    {
        /// <summary>
        /// Liste des films trouvés.
        /// </summary>
        [JsonPropertyName("results")]
        public List<TmdbMovieResult> Results { get; set; } = new();
    }
}
