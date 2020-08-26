using MongoDB.Driver;

namespace Alidu.Core.MongoDB
{
    public class MongoDbContextOptions
    {
        public MongoClientSettings Settings { get; set; }
        public string DatabaseName { get; set; }
    }
}