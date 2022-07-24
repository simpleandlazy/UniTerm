#if UNITY_EDITOR
using SimpleAndLazy.Editor.Public;

namespace SimpleAndLazy.Editor
{
    internal class PowerShell : ShellBase
    {
        public override ShellType GetShellType()
        {
            return ShellType.POWERSHELL;
        }

        public override string[] GetBootCommands()
        {
            return new string[] { };
        }


        public override string GetExecutableName()
        {
            return "powershell.exe";
        }

        public override string GetCurrentWorkpathCommand()
        {
            return "pwd";
        }

        public override string GetClearCommand()
        {
            return "cls";
        }
    }
}
#endif