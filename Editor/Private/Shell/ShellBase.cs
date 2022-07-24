#if UNITY_EDITOR
using SimpleAndLazy.Editor.Public;

namespace SimpleAndLazy.Editor
{
    internal abstract class ShellBase
    {
        public abstract ShellType GetShellType();
        public abstract string[] GetBootCommands();
        public abstract string GetExecutableName();
        public abstract string GetCurrentWorkpathCommand();
        public abstract string GetClearCommand();
    }
}
#endif