
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using Movie_Library.Classes;
using Movie_Library.Data;

namespace Movie_Library.Repositories
{
    /// <summary>
    /// Repository responsable de la gestion des collections.
    /// </summary>
    public class CollectionRepository
    {
        private readonly MongoDbService _mongoDbService;

        public CollectionRepository(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        /// <summary>
        /// Enregistre une collection ou met à jour ses données.
        /// </summary>
        public Task SaveAsync(Collection collection)
        {
            return _mongoDbService.SaveCollectionAsync(collection);
        }

        /// <summary>
        /// Récupère les collections enregistrées.
        /// </summary>
        public Task<List<BsonDocument>> GetAllAsync()
        {
            return _mongoDbService.GetCollectionsAsync();
        }

        /// <summary>
        /// Supprime une collection enregistrée.
        /// </summary>
        public Task<bool> DeleteAsync(string name)
        {
            return _mongoDbService.DeleteCollectionAsync(name);
        }
    }
}