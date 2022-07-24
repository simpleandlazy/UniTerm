#if UNITY_EDITOR
using UnityEngine;

namespace SimpleAndLazy.Editor
{
    internal static class WindowResources
    {
        public static Texture2D AddButtonTexture;
        public static Texture2D CloseButtonTexture;
        public static Texture2D BinButtonTexture;
        public static Texture2D ClipboardButtonTexture;
        public static Texture2D RunButtonTexture;
        public static Texture2D CheckButtonTexture;

        public static GUIContent AddShellButtonContent;
        public static GUIContent CloseShellButtonContent;
        public static GUIContent BinShellButtonContent;
        public static GUIContent ClipboardButtonContent;

        public static GUIContent AddPresetButtonContent;
        public static GUIContent ClosePresetButtonContent;
        public static GUIContent RunPresetButtonContent;
        public static GUIContent CheckPresetButtonContent;

        static WindowResources()
        {
            AddButtonTexture = UnityEngine.Resources.Load<Texture2D>("UniTerm/Texture/addbutton");
            CloseButtonTexture = UnityEngine.Resources.Load<Texture2D>("UniTerm/Texture/closebutton");
            BinButtonTexture = UnityEngine.Resources.Load<Texture2D>("UniTerm/Texture/bin");
            ClipboardButtonTexture = UnityEngine.Resources.Load<Texture2D>("UniTerm/Texture/clipboard");
            RunButtonTexture = UnityEngine.Resources.Load<Texture2D>("UniTerm/Texture/run");
            CheckButtonTexture = UnityEngine.Resources.Load<Texture2D>("UniTerm/Texture/check");

            AddShellButtonContent = new GUIContent(AddButtonTexture,
                $"Add terminal({EditorPlatform.GetPlatfomrControlKeyShortName()}+T)");
            CloseShellButtonContent = new GUIContent(CloseButtonTexture,
                $"Close selected terminal({EditorPlatform.GetPlatfomrControlKeyShortName()}+W)");
            BinShellButtonContent = new GUIContent(BinButtonTexture,
                $"Erase outputs({EditorPlatform.GetPlatfomrControlKeyShortName()}+L)");
            ClipboardButtonContent = new GUIContent(ClipboardButtonTexture, "Copy texts");

            AddPresetButtonContent = new GUIContent(AddButtonTexture,
                $"Add Preset");
            ClosePresetButtonContent = new GUIContent(CloseButtonTexture,
                $"Remove Preset");
            RunPresetButtonContent = new GUIContent(RunButtonTexture,
                $"Run Preset");
            CheckPresetButtonContent = new GUIContent(CheckButtonTexture,
                $"Apply changes");
        }
    }
}
#endif