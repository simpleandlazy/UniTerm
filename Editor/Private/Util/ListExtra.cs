#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;

namespace SimpleAndLazy.Editor
{
    internal static class ListExtra
    {
        public static void Resize<T>(this List<T> list, int sz, T c) where T : new()
        {
            int cur = list.Count;
            if (sz < cur)
                list.RemoveRange(sz, cur - sz);
            else if (sz > cur)
            {
                if (
                    sz > list.Capacity) //this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                    list.Capacity = sz;
                for (int i = 0; i < sz - cur; i++)
                {
                    list.Add(new T());
                }
            }
        }

        public static void Resize<T>(this List<T> list, int sz) where T : new()
        {
            Resize(list, sz, new T());
        }
    }
}
#endif