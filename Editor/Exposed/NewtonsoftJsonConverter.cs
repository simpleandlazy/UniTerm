#if UNITY_EDITOR
using Newtonsoft.Json;

namespace SimpleAndLazy.Editor.Public
{
    public class NewtonsoftJsonConverter : IJsonConvert
    {
        public string SerializeObject(object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        public T DeserializeObject<T>(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }
    }
}
#endif