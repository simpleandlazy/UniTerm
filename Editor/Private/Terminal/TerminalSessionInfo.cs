#if UNITY_EDITOR
using System.Collections.Generic;
using SimpleAndLazy.Editor.Public;

namespace SimpleAndLazy.Editor
{
    internal static class StreamType
    {
        internal const string STDOUT = "stdout";
        internal const string STDERR = "stderr";
        internal const string HIDDEN = "hidden";
    }

    internal class TerminalStreamBlock
    {
        public int intputId;
        public string type;
        public string line;
        public string lineWithoutTag;
    }

    internal class TerminalSessionInfo
    {
        public string id;
        public ShellType shellType;
        public string workingPath = "";
        public string currentInput = "";
        public int currentInputId = 0;
        public List<TerminalStreamBlock> outputBlocks = new List<TerminalStreamBlock>();
    }
}
#endif