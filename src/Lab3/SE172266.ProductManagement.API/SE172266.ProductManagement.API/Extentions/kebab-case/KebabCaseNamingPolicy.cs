using System.Text;
using System.Text.Json;

namespace SE172266.ProductManagement.API.Extentions
{
    public class KebabCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
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
