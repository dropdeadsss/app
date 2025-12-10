using MongoDB.Bson;
using System.Text.Json;

namespace UserService.Services
{
    public static class CollectionOperations
    {
        public static Dictionary<string, object> JsonToDictionary(JsonElement json)
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json.GetRawText());
        }

        public static bool ValidateDictionary<T,Y>(Dictionary<T, Y> dict, List<T> keys)
        {
            if (dict == null)
                return false;

            foreach (var key in keys)
            {
                if (!dict.ContainsKey(key) || dict[key] == null || dict[key].ToString() == string.Empty)
                    return false;
            }

            foreach (var pair in dict)
            {
                if (!keys.Contains(pair.Key))
                    return false;
            }

            return true;
        }
    }
}
