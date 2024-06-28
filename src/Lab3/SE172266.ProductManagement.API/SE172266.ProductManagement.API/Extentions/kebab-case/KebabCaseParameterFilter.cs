using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SE172266.ProductManagement.API.Extentions.kebab_case
{
    public class KebabCaseParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            if (parameter.Description != null)
            {
                parameter.Description = ConvertToKebabCase(parameter.Description);
            }
        }

        private string ConvertToKebabCase(string description)
        {
            if (string.IsNullOrEmpty(description)) return description;

            var newName = new System.Text.StringBuilder(description.Length + 10);
            newName.Append(char.ToLowerInvariant(description[0]));
            for (int i = 1; i < description.Length; i++)
            {
                var c = description[i];
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