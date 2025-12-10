using MongoDB.Bson;
using System.Text.Json;

namespace UserServersService.Services
{
    public static class CollectionOperations
    {
        /// <summary>
        /// dsdsadsa
        /// </summary>
        /// <param name="json">sdsadsa</param>
        /// <returns>dsads</returns>
        public static Dictionary<string, object> JsonToDictionary(JsonElement json)
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json.GetRawText());
        }

        /// <summary>
        /// Проверяет наличие ключей keys<T> в словаре dict<T,Y> и проверяет значения на неравенство null. Если все правильно возвращает true, в остальных случаях false
        /// </summary>
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
