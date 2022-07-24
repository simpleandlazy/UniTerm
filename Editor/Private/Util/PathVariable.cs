#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace SimpleAndLazy.Editor
{
    internal class PathVariable
    {
        public static string ReplacePath(string path)
        {
            var ret = path;
            ret = Path.GetFullPath(ret.Replace("${projectPath}", Etc.GetProjectPath()));
            return ret;
        }
    }
}
#endif