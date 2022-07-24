#if UNITY_EDITOR
using System;
using UnityEditor;

namespace SimpleAndLazy.Editor
{
    internal static class TaskHelper
    {
        public static void DoInmainThread(Action action)
        {
            // EditorApplication.delayCall += () => { action?.Invoke(); };
            EditorUpdater.AddAction(action);
        }
    }
}
#endif