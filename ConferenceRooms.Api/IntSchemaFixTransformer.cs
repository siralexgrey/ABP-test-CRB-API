using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

public class IntSchemaFixTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (schema.Format is "int32" or "int64")
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Pattern = null;
        }
        return Task.CompletedTask;
    }
}