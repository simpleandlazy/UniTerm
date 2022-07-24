#if UNITY_EDITOR
using SimpleAndLazy.Editor.Public;

namespace SimpleAndLazy.Editor
{
    internal abstract class PlatformAPIBase
    {
        public abstract ShellType[] GetAvailableShellTypes();
        public abstract ShellType GetDefaultShellType();
    }
}
#endif