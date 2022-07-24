#if UNITY_EDITOR
using SimpleAndLazy.Editor.Public;

namespace SimpleAndLazy.Editor
{
    internal class Zsh : ShellBase
    {
        public override ShellType GetShellType()
        {
            return ShellType.ZSH;
        }

        public override string[] GetBootCommands()
        {
            return new string[]
            {
                "export PATH=/usr/local/bin:$PATH",
                "source ~/.zshrc"
            };
        }

        public override string GetExecutableName()
        {
            return "/bin/zsh";
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