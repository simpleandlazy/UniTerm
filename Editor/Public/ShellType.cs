namespace SimpleAndLazy.Editor.Public
{
    public enum ShellType
    {
        Unkown = 0,
        CMD = 100,
        POWERSHELL = 101,
        BASH = 200,
        ZSH = 201
    }

    public static class ShellTypeExtensions
    {
        public static string ToAlias(this ShellType shellType)
        {
            switch (shellType)
            {
                case ShellType.CMD:
                    return "cmd";
                case ShellType.POWERSHELL:
                    return "powershell";
                case ShellType.BASH:
                    return "bash";
                case ShellType.ZSH:
                    return "zsh";
            }

            return "unknown";
        }
    }
}