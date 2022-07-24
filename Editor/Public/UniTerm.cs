using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SimpleAndLazy.Editor.Public
{
    public class UniTerm
    {
        private CommandPreset _preset = new CommandPreset
        {
            name = "Empty",
            input = "",
            shellType = ShellType.Unkown,
            workingDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, ".."))
        };

        private string _input = "";

        public string Name
        {
            get => _preset.name;
            set => _preset.name = value;
        }

        public string Input
        {
            get => _input;
            set => _input = value;
        }

        public ShellType ShellType
        {
            get => _preset.shellType;
            set => _preset.shellType = value;
        }

        public string WorkingPath
        {
            get => _preset.workingDirectory;
            set => _preset.workingDirectory = value;
        }

        public UniTerm()
        {
        }

        public async Task<string> Run()
        {
            var window = UniTermWindow.GetInstance();
            var term = window.AddTerminalPreset(_preset);
            while (term.IsProcessing())
            {
                await Task.Delay(10);
            }

            return await term.Input(_input);
        }
    }
}