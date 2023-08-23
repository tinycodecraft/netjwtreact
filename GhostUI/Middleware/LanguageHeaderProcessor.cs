using NSwag.Generation.Processors.Contexts;
using NSwag.Generation.Processors;
using NSwag;

namespace GhostUI.Middleware
{


    public class LanguageHeaderProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var parameter = new OpenApiParameter
            {
                Name = "Accept-Language",
                Kind = OpenApiParameterKind.Header,
                Description = "Language preference for the response.",
                IsRequired = true,
                IsNullableRaw = true,
                Default = "en-US",
                Schema = new NJsonSchema.JsonSchema()
                {
                    Type = NJsonSchema.JsonObjectType.String,
                    Item = new NJsonSchema.JsonSchema() { Type = NJsonSchema.JsonObjectType.String },
                },
            };
            parameter.Schema.Enumeration.Add("en-US");
            //parameter.Schema.Enumeration.Add("fr-FR");
            //parameter.Schema.Enumeration.Add("nl-NL");
            context.OperationDescription.Operation.Parameters.Add(parameter);
            return true;
        }
    }
}
