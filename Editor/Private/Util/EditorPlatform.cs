#if UNITY_EDITOR
using System.Runtime.InteropServices;
using UnityEngine;

namespace SimpleAndLazy.Editor
{
    internal class EditorPlatform
    {
        internal enum Type
        {
            Windows,
            Mac,
            Linux,
        }

        public static Type Get()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Type.Mac;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Type.Linux;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Type.Windows;
            }

            // Debug.LogError("Cannot determine operating system!");
            return Type.Linux;
        }

        public static bool IsPlatformControlKeyPressed(Event eventParam)
        {
            switch (Get())
            {
                case EditorPlatform.Type.Windows:
                case EditorPlatform.Type.Linux:
                    return eventParam.control;
                case EditorPlatform.Type.Mac:
                    return eventParam.command;
                default:
                    return false;
            }
        }

        public static string GetPlatfomrControlKeyShortName()
        {
            switch (Get())
            {
                case EditorPlatform.Type.Windows:
                case EditorPlatform.Type.Linux:
                    return "Ctrl";
                case EditorPlatform.Type.Mac:
                    return "Cmd";
                default:
                    return "?";
            }
        }
    }
}
#endif