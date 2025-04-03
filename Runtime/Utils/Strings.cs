using System.Collections.Generic;
using System.Text;

namespace BIG
{
    public static class Strings
    {
        public static string DictionaryToJsonString(this Dictionary<string, object> dict)
        {
            if (dict == null || dict.Count == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder(dict.Count);
            sb.AppendLine("{");
            foreach (KeyValuePair<string, object> pair in dict)
            {
                sb.AppendLine($"    \"{pair.Key}\" : \"{pair.Value}\",");
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
