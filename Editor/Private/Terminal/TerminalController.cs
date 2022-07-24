#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using SimpleAndLazy.Editor.Public;
using UnityEditor;

namespace SimpleAndLazy.Editor
{
    internal class TerminalController
    {
        public IReadOnlyList<Terminal> Terminals => terminals.AsReadOnly();
        public Action OnCommandProcessed;
        private List<Terminal> terminals = new List<Terminal>();
        internal const string META_STATE_KEY = "SIMTERM_META_STATE_KEY";
        internal const string SESSION_STATE_KEY = "SIMTERM_SESSION_STATE_KEY";


        public TerminalController()
        {
            Load();
        }

        public static void RemoveAll()
        {
            EditorPrefs.DeleteKey(META_STATE_KEY);
            EditorPrefs.DeleteKey(SESSION_STATE_KEY);
        }

        public Terminal AddTerminal(ShellType shellType)
        {
            var newTerm = new Terminal(shellType);
            newTerm.OnCommandProcessed = () => OnTerminalCommandProcessed(newTerm);
            terminals.Add(newTerm);
            Save(newTerm);
            return newTerm;
        }

        public Terminal AddTerminal(CommandPreset preset)
        {
            if (preset.shellType == ShellType.Unkown) preset.shellType = PlatformAPI.Get().GetDefaultShellType();
            if (!PlatformAPI.Get().GetAvailableShellTypes().Contains(preset.shellType))
                preset.shellType = PlatformAPI.Get().GetDefaultShellType();

            var newTerm = new Terminal(preset);
            newTerm.OnCommandProcessed = () => OnTerminalCommandProcessed(newTerm);
            terminals.Add(newTerm);
            Save(newTerm);
            return newTerm;
        }

        public void RemoveTerminal(Terminal terminal)
        {
            terminal.Stop();
            terminals.Remove(terminal);
            Save(null);
        }

        public void ClearOutput(Terminal terminal)
        {
            terminal.GetSessionInfo().outputBlocks.Clear();
            Save(null);
        }

        private void Save(Terminal terminal)
        {
            {
                List<string> sessionIds = new List<string>();
                terminals.ForEach((t) => sessionIds.Add(t.GetSessionInfo().id));
                string serialized = JsonConvert.Instance.SerializeObject(sessionIds);
                EditorPrefs.SetString(META_STATE_KEY, serialized);
            }
            {
                terminals.ForEach((t) =>
                {
                    string serialized = JsonConvert.Instance.SerializeObject(t.GetSessionInfo());
                    EditorPrefs.SetString($"{SESSION_STATE_KEY}_{t.GetSessionInfo().id}", serialized);
                });
            }
        }

        private void Load()
        {
            {
                var serialized = EditorPrefs.GetString(META_STATE_KEY, "");
                if (string.IsNullOrEmpty(serialized)) return;

                List<string> sessionIds = JsonConvert.Instance.DeserializeObject<List<string>>(serialized);
                foreach (var sid in sessionIds)
                {
                    var termianlSerialized =
                        EditorPrefs.GetString($"{SESSION_STATE_KEY}_{sid}", "");
                    if (string.IsNullOrEmpty(termianlSerialized)) continue;

                    TerminalSessionInfo session =
                        JsonConvert.Instance.DeserializeObject<TerminalSessionInfo>(termianlSerialized);

                    var newTerm = new Terminal(session);
                    newTerm.OnCommandProcessed = () => OnTerminalCommandProcessed(newTerm);
                    terminals.Add(newTerm);
                }
            }
        }

        private void OnTerminalCommandProcessed(Terminal terminal)
        {
            Save(terminal);
            OnCommandProcessed?.Invoke();
        }
    }
}
#endif