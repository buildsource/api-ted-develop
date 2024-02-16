using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using TED.API.Annotations;

namespace TED.API.Filters;

public class IncludeInSwaggerDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var includedTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => type.GetCustomAttribute<IncludeInSwaggerAttribute>() != null);

        foreach (var includedType in includedTypes)
        {
            var schemaId = includedType.FullName;
            if (!swaggerDoc.Components.Schemas.ContainsKey(schemaId))
            {
                var dtoSchema = context.SchemaGenerator.GenerateSchema(includedType, context.SchemaRepository);
                swaggerDoc.Components.Schemas.Add(schemaId, dtoSchema);
            }
        }
    }
}