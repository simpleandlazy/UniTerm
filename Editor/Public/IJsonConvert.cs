namespace SimpleAndLazy.Editor.Public
{
    public interface IJsonConvert
    {
        string SerializeObject(object value);
        T DeserializeObject<T>(string value);
    }
}