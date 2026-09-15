using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Movie_Library.Classes;

namespace Movie_Library.Data
{
    /// <summary>
    /// Service gérant l'accès unique à la base de données MongoDB.
    /// </summary>
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Constructeur qui initialise la connexion.
        /// </summary>
        public MongoDbService(IConfiguration configuration)
        {
            string connectionString = configuration.GetValue<string>("MongoDbSettings:ConnectionString");
            string databaseName = configuration.GetValue<string>("MongoDbSettings:DatabaseName");

            MongoClient client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        /// <summary>
        /// Collection des films.
        /// </summary>
        public IMongoCollection<Movie> Movies
        {
            get { return _database.GetCollection<Movie>("movies"); }
        }

        /// <summary>
        /// Collection des groupes/collections de films.
        /// </summary>
        public IMongoCollection<Collection> Collections
        {
            get { return _database.GetCollection<Collection>("collections"); }
        }
    }
}
