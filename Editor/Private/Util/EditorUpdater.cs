#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace SimpleAndLazy.Editor
{
    [InitializeOnLoad]
    internal class EditorUpdater : EditorTool
    {
        private static List<Action> actions = new List<Action>();

        static EditorUpdater()
        {
            EditorApplication.update += Update;
        }

        public static void AddAction(Action act)
        {
            actions.Add(act);
        }

        static void Update()
        {
            try
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    actions[i]?.Invoke();
                }
            }
            catch (Exception)
            {
                // Debug.LogError("SimpleAndLazy.Editor.EditorUpdater failed");
            }

            actions.Clear();
        }
    }
}
#endif