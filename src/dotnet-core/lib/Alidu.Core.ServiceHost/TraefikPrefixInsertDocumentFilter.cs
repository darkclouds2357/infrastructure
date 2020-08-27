using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alidu.Core.ServiceHost
{
    public class TraefikPrefixInsertDocumentFilter : IDocumentFilter
    {
        private readonly string _prefix;

        public TraefikPrefixInsertDocumentFilter(string prefix)
        {
            _prefix = prefix;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths.Keys.ToList();
            foreach (var path in paths)
            {
                var pathToChange = swaggerDoc.Paths[path];
                swaggerDoc.Paths.Remove(path);
                swaggerDoc.Paths.Add($"/{_prefix}{path}", pathToChange);
            }
        }
    }
}
