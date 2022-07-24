#if UNITY_EDITOR
using System;
using System.Linq;
using SimpleAndLazy.Editor.Public;

namespace SimpleAndLazy.Editor
{
    internal static class JsonConvert
    {
        public static IJsonConvert Instance => instance;
        private static IJsonConvert instance;

        static JsonConvert()
        {
            var type = typeof(IJsonConvert);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s =>
                {
                    try
                    {
                        var t = s.GetTypes();
                        return t;
                    }
                    catch (Exception)
                    {
                        return new Type[] { };
                    }
                })
                .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);
            if (!types.Any()) return;
            var target = types.ToArray()[0];
            instance = Activator.CreateInstance(target) as IJsonConvert;
        }
    }
}
#endif