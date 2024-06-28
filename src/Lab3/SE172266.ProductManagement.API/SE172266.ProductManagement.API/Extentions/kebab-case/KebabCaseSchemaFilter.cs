using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace SE172266.ProductManagement.API.Extentions.kebab_case
{
    public class KebabCaseSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null) return;

            var newProperties = new Dictionary<string, OpenApiSchema>();
            foreach (var property in schema.Properties.ToList())
            {
                var kebabCasePropertyName = ConvertToKebabCase(property.Key);
                schema.Properties.Remove(property.Key);
                schema.Properties.Add(kebabCasePropertyName, property.Value);
            }
        }

        private string ConvertToKebabCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;

            var newName = new System.Text.StringBuilder(name.Length + 10);
            newName.Append(char.ToLowerInvariant(name[0]));
            for (int i = 1; i < name.Length; i++)
            {
                var c = name[i];
                if (char.IsUpper(c))
                {
                    newName.Append('-');
                    newName.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    newName.Append(c);
                }
            }
            return newName.ToString();
        }
    }
}
