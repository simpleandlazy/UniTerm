#if UNITY_EDITOR
using System;
using SimpleAndLazy.Editor.Public;

namespace SimpleAndLazy.Editor
{
    internal class Shell
    {
        public static ShellBase Create(ShellType shellType)
        {
            switch (shellType)
            {
                case ShellType.CMD:
                    return new Cmd();
                case ShellType.POWERSHELL:
                    return new PowerShell();
                case ShellType.BASH:
                    return new Bash();
                case ShellType.ZSH:
                    return new Zsh();
                default:
                    throw new Exception($"Shell.Create Unable to create shell {shellType}");
            }
        }
    }
}
#endif