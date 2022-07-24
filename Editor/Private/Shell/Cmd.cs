#if UNITY_EDITOR
using SimpleAndLazy.Editor.Public;

namespace SimpleAndLazy.Editor
{
    internal class Cmd : ShellBase
    {
        public override ShellType GetShellType()
        {
            return ShellType.CMD;
        }

        public override string[] GetBootCommands()
        {
            return new string[] { };
        }


        public override string GetExecutableName()
        {
            return "cmd.exe";
        }

        public override string GetCurrentWorkpathCommand()
        {
            return "cd ,";
        }

        public override string GetClearCommand()
        {
            return "cls";
        }
    }
}
#endif