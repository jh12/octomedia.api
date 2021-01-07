using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OctoMedia.Api.Common.Options;

namespace OctoMedia.Api.DataAccess.MongoDB.Factories
{
    public class MongoDBContextFactory
    {
        private readonly MongoDBOptions _mongoDbOptions;

        public MongoDBContextFactory(IOptions<MongoDBOptions> mongoDbOptions)
        {
            _mongoDbOptions = mongoDbOptions.Value;
        }

        public IMongoDatabase GetDatabase()
        {
            MongoClient client = new MongoClient(new MongoClientSettings
            {
                Server = MongoServerAddress.Parse(_mongoDbOptions.Server),
                Credential = MongoCredential.CreateCredential(_mongoDbOptions.Database, _mongoDbOptions.Username, _mongoDbOptions.Password)
            });

            return client.GetDatabase(_mongoDbOptions.Database);
        }

        public IMongoCollection<T> GetContext<T>(string collectionName)
        {
            IMongoDatabase database = GetDatabase();

            return database.GetCollection<T>(collectionName);
        }
    }
}