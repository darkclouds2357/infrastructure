using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.MongoDB
{
    public class MongoDbContextOptions
    {
        public MongoClientSettings Settings { get; set; }
        public string DatabaseName { get; set; }
    }
}
