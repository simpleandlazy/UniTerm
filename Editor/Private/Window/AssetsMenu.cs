#if UNITY_EDITOR

using System.IO;
using SimpleAndLazy.Editor.Public;
using UnityEditor;
using UnityEngine;

namespace SimpleAndLazy.Editor
{
    internal class AssetsMenu
    {
        [MenuItem("Assets/Open in UniTerm")]
        static void Open()
        {
            var selectedPath = Path.Combine(Application.dataPath, "..", Etc.GetSelectedPathOrFallback());
            var window = EditorWindow.GetWindow<UniTermWindow>();
            var newPreset = new CommandPreset
            {
                name = "AssetsMenu",
                shellType = window.GetSelectedShellType(),
                workingDirectory = selectedPath,
                input = ""
            };
            window.AddTerminalPreset(newPreset);
        }
    }
}
#endif