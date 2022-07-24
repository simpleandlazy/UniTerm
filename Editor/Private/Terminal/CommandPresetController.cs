#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using SimpleAndLazy.Editor.Public;
using UnityEngine;

namespace SimpleAndLazy.Editor
{
    internal class CommandPresetController
    {
        public IReadOnlyList<CommandPreset> Presets => presets.AsReadOnly();
        private List<CommandPreset> presets = new List<CommandPreset>();
        private static string DirPath = $"{Application.dataPath}/UniTerm";
        private static string FilePath = $"{DirPath}/commandPresets.json";

        public CommandPresetController()
        {
            Load();
        }

        public void AddPreset(CommandPreset commandPreset)
        {
            presets.Add(commandPreset);
            Save();
        }

        public void RemovePreset(CommandPreset commandPreset)
        {
            presets.Remove(commandPreset);
            Save();
        }


        public void Load()
        {
            if (!File.Exists(FilePath)) return;
            string serizliaed = File.ReadAllText(FilePath);
            presets = JsonConvert.Instance.DeserializeObject<List<CommandPreset>>(serizliaed);
        }

        public void Save()
        {
            string serialized = JsonConvert.Instance.SerializeObject(presets);
            if (!Directory.Exists(DirPath)) Directory.CreateDirectory(DirPath);
            File.WriteAllText(FilePath, serialized);
        }
    }
}
#endif