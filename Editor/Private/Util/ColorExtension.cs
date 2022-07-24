#if UNITY_EDITOR
using UnityEngine;

namespace SimpleAndLazy.Editor
{
    internal static class ColorExtension
    {
        public static string ToRichTextTag(this Color color)
        {
            return $"<color=#{ToHex(color.r)}{ToHex(color.g)}{ToHex(color.b)}>";
        }

        private static string ToHex(float colorElem)
        {
            int a = (int) (colorElem * 255.0f);
            return a.ToString("x2");
        }
    }
}
#endif