#if UNITY_EDITOR
using SimpleAndLazy.Editor.Public;

namespace SimpleAndLazy.Editor
{
    internal class UnixPlatformAPI : PlatformAPIBase
    {
        private ShellType[] availableTypes = new ShellType[] {ShellType.BASH, ShellType.ZSH};

        public override ShellType[] GetAvailableShellTypes()
        {
            return availableTypes;
        }

        public override ShellType GetDefaultShellType()
        {
            return ShellType.ZSH;
        }
    }
}
#endif