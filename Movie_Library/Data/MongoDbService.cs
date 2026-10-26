using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Movie_Library.Classes;

namespace Movie_Library.Data
{
    /// <summary>
    /// Service responsable de l'accès à MongoDB.
    /// Il gère l'ajout, la récupération, la modification
    /// et la suppression des films.
    /// </summary>
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<BsonDocument> _movies;
       
        // Accès aux collections personnelles enregistrées dans MongoDB.
        private readonly IMongoCollection<BsonDocument> _collections;

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

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "La chaîne de connexion MongoDB est manquante."
                );
            }

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new InvalidOperationException(
                    "Le nom de la base de données MongoDB est manquant."
                );
            }

            MongoClient client = new MongoClient(connectionString);

            _database = client.GetDatabase(databaseName);

            _movies = _database.GetCollection<BsonDocument>("movies");

            _collections = _database.GetCollection<BsonDocument>("collections");
        }

        /// <summary>
        /// Vérifie que MongoDB répond correctement.
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using var cursor = _database.ListCollectionNames();
                cursor.MoveNext();

                return true;
            }
            catch (MongoException ex)
            {
                Console.WriteLine(
                    $"Erreur de connexion MongoDB : {ex.Message}"
                );

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Erreur inattendue MongoDB : {ex.Message}"
                );

                return false;
            }
        }

        /// <summary>
        /// Vérifie si un film existe déjà dans MongoDB.
        /// L'identifiant TMDB est utilisé comme _id.
        /// </summary>
        public async Task<bool> MovieExistsAsync(int tmdbId)
        {
            var filter = Builders<BsonDocument>.Filter
                .Eq("_id", tmdbId);

            return await _movies
                .Find(filter)
                .AnyAsync();
        }

        /// <summary>
        /// Ajoute un film dans MongoDB.
        /// Le TMDB ID est utilisé comme _id MongoDB.
        ///
        /// Retourne true si le film est ajouté,
        /// false si un film avec cet identifiant existe déjà.
        /// </summary>
        public async Task<bool> AddMovieAsync(Movie movie)
        {
            try
            {
                BsonDocument document = new BsonDocument
                {
                    { "_id", movie.Id },
                    { "title", movie.Title },
                    { "synopsis", movie.Synopsis ?? string.Empty },
                    { "poster", movie.Poster ?? string.Empty },
                    { "ratingTMDB", (double)movie.RatingTMDB },
                    { "personalRating", (double)movie.PersonalRating },
                    { "personalNote", movie.PersonalNote ?? string.Empty },
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
            List<BsonDocument> documents = await _movies
                .Find(FilterDefinition<BsonDocument>.Empty)
                .ToListAsync();

            List<Movie> movies = new();

            foreach (BsonDocument document in documents)
            {
                int id = document.GetValue("_id").AsInt32;

                string title = document
                    .GetValue("title", "")
                    .AsString;

                string synopsis = document
                    .GetValue("synopsis", "")
                    .AsString;

                string poster = document
                    .GetValue("poster", "")
                    .AsString;

                float ratingTMDB = (float)document
                    .GetValue("ratingTMDB", 0)
                    .ToDouble();

                string statusValue = document
                    .GetValue("status", "NotSeen")
                    .AsString;

                Status status = Status.NotSeen;

                if (!Enum.TryParse(statusValue, true, out status)
                    || !Enum.IsDefined(typeof(Status), status))
                {
                    status = Status.NotSeen;
                }

                Movie movie = new Movie(
                    id,
                    title,
                    synopsis,
                    poster,
                    ratingTMDB,
                    status
                );

                if (document.Contains("personalRating"))
                {
                    movie.PersonalRating = (float)document
                        .GetValue("personalRating")
                        .ToDouble();
                }

                if (document.Contains("personalNote"))
                {
                    movie.PersonalNote = document
                        .GetValue("personalNote")
                        .AsString;
                }

                movies.Add(movie);
            }

            return movies;
        }

        /// <summary>
        /// Met à jour le statut de visionnage d'un film.
        /// </summary>
        public async Task<bool> UpdateMovieStatusAsync(
            int tmdbId,
            Status status)
        {
            var filter = Builders<BsonDocument>.Filter
                .Eq("_id", tmdbId);

            var update = Builders<BsonDocument>.Update
                .Set("status", status.ToString());

            var result = await _movies.UpdateOneAsync(
                filter,
                update
            );

            return result.MatchedCount > 0;
        }

        /// <summary>
        /// Enregistre la note et le commentaire personnels.
        /// </summary>
        public async Task<bool> UpdatePersonalDataAsync(
            int tmdbId,
            float personalRating,
            string? personalNote)
        {
            var filter = Builders<BsonDocument>.Filter
                .Eq("_id", tmdbId);

            var update = Builders<BsonDocument>.Update
                .Set("personalRating", (double)personalRating)
                .Set("personalNote", personalNote ?? string.Empty);

            var result = await _movies.UpdateOneAsync(
                filter,
                update
            );

            return result.MatchedCount > 0;
        }

        /// <summary>
        /// Met à jour le statut et les données personnelles
        /// d'un film en une seule opération.
        /// Cette méthode est utilisée par la modale de gestion.
        /// </summary>
        public async Task<bool> UpdateMovieDetailsAsync(
            int tmdbId,
            Status status,
            float personalRating,
            string? personalNote)
        {
            if (personalRating < 0 || personalRating > 10)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(personalRating),
                    "La note personnelle doit être comprise entre 0 et 10."
                );
            }

            var filter = Builders<BsonDocument>.Filter
                .Eq("_id", tmdbId);

            var update = Builders<BsonDocument>.Update
                .Set("status", status.ToString())
                .Set("personalRating", (double)personalRating)
                .Set("personalNote", personalNote ?? string.Empty);

            var result = await _movies.UpdateOneAsync(
                filter,
                update
            );

            return result.MatchedCount > 0;
        }

        /// <summary>
        /// Supprime un film de la bibliothèque MongoDB.
        /// </summary>
        public async Task<bool> DeleteMovieAsync(int tmdbId)
        {
            var filter = Builders<BsonDocument>.Filter
                .Eq("_id", tmdbId);

            var result = await _movies.DeleteOneAsync(filter);

            return result.DeletedCount > 0;
        }

        /// <summary>
        /// Enregistre une collection ou met à jour ses informations.
        /// Les films sont identifiés par leur identifiant TMDB.
        /// </summary>
        public async Task SaveCollectionAsync(Collection collection)
        {
            var filter = Builders<BsonDocument>.Filter
                .Eq("name", collection.Name);

            var document = new BsonDocument
    {
        { "name", collection.Name },
        { "poster", collection.Poster ?? string.Empty },
        {
            "movieIds",
            new BsonArray(
                collection.Movies.Select(movie => movie.Id)
            )
        }
    };

            await _collections.ReplaceOneAsync(
                filter,
                document,
                new ReplaceOptions { IsUpsert = true }
            );
        }

        /// <summary>
        /// Récupère les collections enregistrées dans MongoDB.
        /// </summary>
        public async Task<List<BsonDocument>> GetCollectionsAsync()
        {
            return await _collections
                .Find(FilterDefinition<BsonDocument>.Empty)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère les collections enregistrées dans MongoDB
        /// et reconstitue leurs listes de films.
        /// </summary>
        public async Task<List<Collection>> LoadCollectionsAsync(IEnumerable<Movie> movies)
        {
            // Récupère toutes les collections enregistrées dans MongoDB.
            List<BsonDocument> documents = await GetCollectionsAsync();


            // Prépare la liste des collections à reconstituer.
            List<Collection> collections = new();

            // Parcourt chaque document de collection récupéré.
            foreach (BsonDocument document in documents)
            {
                // Récupère le nom de la collection.
                string name = document.GetValue("name", "").AsString;

                // Récupère son affiche.
                string poster = document.GetValue("poster", "").AsString;

                // Crée un nouvel objet Collection.
                Collection collection = new Collection(name, poster);

                // Vérifie si le document contient des identifiants de films.
                if (document.Contains("movieIds"))
                {
                    // Parcourt les identifiants des films de cette collection.
                    foreach (BsonValue movieId in document["movieIds"].AsBsonArray)
                    {
                        // Recherche le film correspondant dans la bibliothèque.
                        Movie? movie = movies.FirstOrDefault(
                            m => m.Id == movieId.ToInt32()
                        );

                        // Ajoute le film s'il existe.
                        if (movie != null)
                        {
                            collection.addMovie(movie);
                        }
                    }
                }

                // Ajoute la collection reconstituée à la liste.
                collections.Add(collection);
            }

            // Retourne toutes les collections reconstituées.
            return collections;
        }

        /// <summary>
        /// Supprime une collection à partir de son nom.
        /// </summary>
        public async Task<bool> DeleteCollectionAsync(string name)
        {
            var filter = Builders<BsonDocument>.Filter
                .Eq("name", name);

            var result = await _collections.DeleteOneAsync(filter);

            return result.DeletedCount > 0;
        }
    }
}