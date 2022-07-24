#if UNITY_EDITOR
using SimpleAndLazy.Editor.Public;

namespace SimpleAndLazy.Editor
{
    internal class Bash : ShellBase
    {
        public override ShellType GetShellType()
        {
            return ShellType.BASH;
        }

        public override string[] GetBootCommands()
        {
            return new string[]
            {
                "export PATH=/usr/local/bin:$PATH",
                "source ~/.bash_profile"
            };
        }

        public override string GetExecutableName()
        {
            return "/bin/bash";
        }

        public override string GetCurrentWorkpathCommand()
        {
            return "pwd";
        }

        public override string GetClearCommand()
        {
            return "clear";
        }
    }
}
#endif