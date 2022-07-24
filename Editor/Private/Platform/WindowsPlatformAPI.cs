#if UNITY_EDITOR
using SimpleAndLazy.Editor.Public;

namespace SimpleAndLazy.Editor
{
    internal class WindowsPlatformAPI : PlatformAPIBase
    {
        private ShellType[] availableTypes = new ShellType[] {ShellType.CMD, ShellType.POWERSHELL};

        public override ShellType[] GetAvailableShellTypes()
        {
            return availableTypes;
        }

        public override ShellType GetDefaultShellType()
        {
            return ShellType.CMD;
        }
    }
}
#endif