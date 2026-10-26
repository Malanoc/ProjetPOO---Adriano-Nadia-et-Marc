using System.Collections.Generic;
using System.Threading.Tasks;
using Movie_Library.Classes;
using Movie_Library.Data;

namespace Movie_Library.Repositories
{
    /// <summary>
    /// Repository responsable de la gestion des films qui utilise MongoDbService pour accéder à MongoDB.
    /// </summary>
    public class MovieRepository
    {
        private readonly MongoDbService _mongoDbService;

        public MovieRepository(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        /// <summary>
        /// Récupère tous les films enregistrés.
        /// </summary>
        public Task<List<Movie>> GetAllAsync()
        {
            return _mongoDbService.GetMoviesAsync();
        }

        /// <summary>
        /// Vérifie si un film existe.
        /// </summary>
        public Task<bool> ExistsAsync(int tmdbId)
        {
            return _mongoDbService.MovieExistsAsync(tmdbId);
        }

        /// <summary>
        /// Ajoute un film.
        /// Retourne false si le film existe déjà.
        /// </summary>
        public Task<bool> AddAsync(Movie movie)
        {
            return _mongoDbService.AddMovieAsync(movie);
        }

        /// <summary>
        /// Modifie le statut et les données personnelles.
        /// </summary>
        public Task<bool> UpdateDetailsAsync(
            int tmdbId,
            Status status,
            float personalRating,
            string? personalNote)
        {
            return _mongoDbService.UpdateMovieDetailsAsync(
                tmdbId,
                status,
                personalRating,
                personalNote
            );
        }

        /// <summary>
        /// Supprime un film.
        /// </summary>
        public Task<bool> DeleteAsync(int tmdbId)
        {
            return _mongoDbService.DeleteMovieAsync(tmdbId);
        }
    }
}

