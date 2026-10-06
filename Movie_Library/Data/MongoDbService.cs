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
        // Varaible privée qui stocke la référence à notre BD
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Constructeur qui initialise la connexion.
        /// </summary>
        public MongoDbService(IConfiguration configuration)
        {
            //1. On récupère la chaîne de caractère depuis appsettings.json
            string? connectionString = configuration.GetValue<string>("MongoDbSettings:ConnectionString");
            string? databaseName = configuration.GetValue<string>("MongoDbSettings:DatabaseName");
            //2. On initialise le client MongoDB
            MongoClient client = new MongoClient(connectionString);

            //3. Connexion à la BD spécifique (et créée la BD si elle n'existe pas)
            _database = client.GetDatabase(databaseName);
        }


        /// <summary>
        /// Vérfie que MongoDB répond correctement
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                // On essaie de récupérer la liste des collections de notre base
                // Si MongoDB est éteint, cette ligne va planter et aller directement dans le "catch".
                _database.ListCollectionNames();

                // Sinon c'est que la connexion fonctionne
                return true;
            }
            // Erreur si le serveur MongoDB est éteint
            catch (TimeoutException)
            {
                Console.WriteLine("Le serveur MonogDB ne repond pas (Timeout). Est-il démarré?");
                return false;

            }
            // Attrape tout autre erreur
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur MongoDB inconnue:{ex.Message}");
                
                return false;
            }
        }

    }
}