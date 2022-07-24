#if UNITY_EDITOR
using UnityEngine;

namespace SimpleAndLazy.Editor
{
    internal static class WindowStyles
    {
        // Topbar
        public static GUIStyle Style_TopbarLabel;

        public static GUILayoutOption[] Layouts_TopbarLabel = new[]
            {GUILayout.Width(75.0f), GUILayout.ExpandHeight(false)};

        public static GUILayoutOption[] Layouts_Dropdown = new[]
            {GUILayout.ExpandWidth(true), GUILayout.MinHeight(20.0f)};

        public static GUILayoutOption[] Layouts_SmallButton = new[] {GUILayout.Width(20.0f), GUILayout.Height(20.0f)};

        // TerminalName
        public static GUILayoutOption[] Layouts_TerminalNameScrollView =
            new[] {GUILayout.ExpandWidth(true), GUILayout.Height(35.0f)};

        public static GUIStyle Style_TerminalNamesScrollbar;
        public static GUIStyle Style_SelectedButton;
        public static GUIStyle Style_NonSelectedButton;


        // input output
        public static GUIStyle Style_TerminalInput;

        public static GUILayoutOption[] LayOuts_TerminalInput =
            new[] {GUILayout.ExpandWidth(true)};

        public static GUILayoutOption[] LayOuts_HIDDEN =
            new[] {GUILayout.Width(0.0f), GUILayout.Height(0.0f)};

        public static GUILayoutOption[] Layouts_TerminalScrollView =
            new[] {GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)};

        public static GUILayoutOption[] TerminalOutputSize =
            new[] {GUILayout.ExpandWidth(true)};

        public static GUIStyle Style_TerminalOutput;
        public static GUIStyle Style_TerminalOutputClipboardLabel;

        static WindowStyles()
        {
            Style_TopbarLabel = new GUIStyle(GUI.skin.label);
            Style_TopbarLabel.alignment = TextAnchor.MiddleCenter;
            Style_TerminalNamesScrollbar = new GUIStyle(GUI.skin.horizontalScrollbar);
            Style_SelectedButton = new GUIStyle(GUI.skin.textField);
            Style_SelectedButton.alignment = TextAnchor.MiddleCenter;
            Style_NonSelectedButton = new GUIStyle(GUI.skin.button);
            Style_TerminalInput = new GUIStyle(GUI.skin.textField);
            Style_TerminalInput.fontStyle = FontStyle.Normal;
            Style_TerminalInput.wordWrap = true;
            Style_TerminalOutput = new GUIStyle(GUI.skin.button);
            Style_TerminalOutput.richText = true;
            Style_TerminalOutput.alignment = TextAnchor.UpperLeft;
            Style_TerminalOutput.wordWrap = true;
            Style_TerminalOutputClipboardLabel = new GUIStyle(GUI.skin.button);
        }
    }
}
#endif