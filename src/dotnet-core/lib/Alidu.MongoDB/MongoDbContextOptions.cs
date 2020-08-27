using MongoDB.Driver;

namespace Alidu.MongoDB
{
    public class MongoDbContextOptions
    {
        public MongoClientSettings Settings { get; set; }
        public string DatabaseName { get; set; }
    }
}