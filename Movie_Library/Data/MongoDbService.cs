using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Library.Classes;

namespace Movie_Library.Data
{
    /// <summary>
    /// Service responsable de l'accès à MongoDB.
    /// </summary>
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        private readonly IMongoCollection<BsonDocument> _movies;

        /// <summary>
        /// Initialise la connexion à MongoDB.
        /// </summary>
        public MongoDbService(IConfiguration configuration)
        {
            string? connectionString =
                configuration.GetValue<string>(
                    "MongoDbSettings:ConnectionString"
                );

            string? databaseName =
                configuration.GetValue<string>(
                    "MongoDbSettings:DatabaseName"
                );

            MongoClient client =
                new MongoClient(connectionString);

            _database =
                client.GetDatabase(databaseName);

            _movies =
                _database.GetCollection<BsonDocument>("movies");
        }


        /// <summary>
        /// Vérifie que MongoDB répond correctement.
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                _database.ListCollectionNames();

                return true;
            }
            catch (TimeoutException)
            {
                Console.WriteLine(
                    "Le serveur MongoDB ne répond pas (Timeout). " +
                    "Est-il démarré ?"
                );

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Erreur MongoDB inconnue : {ex.Message}"
                );

                return false;
            }
        }


        /// <summary>
        /// Vérifie si un film existe déjà dans MongoDB.
        /// L'identifiant utilisé est le TMDB ID.
        /// </summary>
        public async Task<bool> MovieExistsAsync(int tmdbId)
        {
            FilterDefinition<BsonDocument> filter =
                Builders<BsonDocument>.Filter.Eq(
                    "_id",
                    tmdbId
                );

            return await _movies
                .Find(filter)
                .AnyAsync();
        }


        /// <summary>
        /// Ajoute un film dans MongoDB.
        /// Le TMDB ID est utilisé comme _id MongoDB.
        /// </summary>
        /// <returns>
        /// true si le film a été ajouté,
        /// false s'il existait déjà.
        /// </returns>
        public async Task<bool> AddMovieAsync(Movie movie)
        {
            try
            {
                BsonDocument document =
                    new BsonDocument
                    {
                        { "_id", movie.Id },
                        { "title", movie.Title },
                        { "synopsis", movie.Synopsis },
                        { "poster", movie.Poster },
                        { "ratingTMDB", (double)movie.RatingTMDB },
                        { "personalRating", (double)movie.PersonalRating },
                        { "personalNote", movie.PersonalNote },
                        { "status", movie.Status.ToString() }
                    };

                await _movies.InsertOneAsync(document);

                return true;
            }
            catch (MongoWriteException ex)
                when (ex.WriteError?.Category ==
                      ServerErrorCategory.DuplicateKey)
            {
                return false;
            }
        }


        /// <summary>
        /// Récupère tous les films enregistrés dans MongoDB.
        /// </summary>
        public async Task<List<Movie>> GetMoviesAsync()
        {
            List<BsonDocument> documents =
                await _movies
                    .Find(FilterDefinition<BsonDocument>.Empty)
                    .ToListAsync();

            List<Movie> movies = new();

            foreach (BsonDocument document in documents)
            {
                int id =
                    document
                        .GetValue("_id")
                        .AsInt32;

                string title =
                    document
                        .GetValue("title", "")
                        .AsString;

                string synopsis =
                    document
                        .GetValue("synopsis", "")
                        .AsString;

                string poster =
                    document
                        .GetValue("poster", "")
                        .AsString;

                float ratingTMDB =
                    (float)document
                        .GetValue("ratingTMDB", 0)
                        .ToDouble();

                Status status = Status.NotSeen;

                string statusValue =
                    document
                        .GetValue("status", "NotSeen")
                        .AsString;

                Enum.TryParse(
                    statusValue,
                    out status
                );

                Movie movie =
                    new Movie(
                        id,
                        title,
                        synopsis,
                        poster,
                        ratingTMDB,
                        status
                    );

                if (document.Contains("personalRating"))
                {
                    movie.PersonalRating =
                        (float)document
                            .GetValue("personalRating")
                            .ToDouble();
                }

                if (document.Contains("personalNote"))
                {
                    movie.PersonalNote =
                        document
                            .GetValue("personalNote")
                            .AsString;
                }

                movies.Add(movie);
            }

            return movies;
        }
    }
}