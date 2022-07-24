#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAndLazy.Editor.Public;
using UnityEditor;
using UnityEngine;

namespace SimpleAndLazy.Editor
{
    internal class Terminal : IUniTermInternal
    {
        public string Name
        {
            get
            {
                return
                    $"{WorkingPathShort}${_sessionInfo.currentInput.Split(' ').First()} - {_sessionInfo.shellType.ToAlias()}";
            }
        }

        public string WorkingPath => _sessionInfo.workingPath;

        public string WorkingPathShort
        {
            get { return _sessionInfo.workingPath.Replace("\\", "/").Split('/').Last(); }
        }

        public List<TerminalStreamBlock> OutputBlocks => _sessionInfo.outputBlocks;

        // command
        public Action OnCommandProcessed;
        public bool IsCommandProcessing = false;
        private Action _onCommandCompleted;

        private TerminalSessionInfo _sessionInfo = new TerminalSessionInfo();
        private System.Diagnostics.Process _process;
        private ShellBase _shell;

        private const string ENV_KEY = "IS_UNITY_SIMTERM_CHILD";
        private const string COMMAND_END_TOKEN = "SIMTERM_END";

        private List<int> _hiddenOutputInputIds = new List<int>();

        // parser
        private XtermParser _parser = new XtermParser();

        public Terminal(ShellType shellType)
        {
            TerminalSessionInfo newSession = new TerminalSessionInfo();
            newSession.id = System.Guid.NewGuid().ToString();
            newSession.shellType = shellType;
            newSession.workingPath = Path.GetFullPath($"{Application.dataPath}/..");
            Create(newSession);
        }

        public Terminal(TerminalSessionInfo sessionInfo)
        {
            Create(sessionInfo);
        }

        public Terminal(CommandPreset preset)
        {
            TerminalSessionInfo newSession = new TerminalSessionInfo();
            newSession.id = System.Guid.NewGuid().ToString();
            newSession.shellType = preset.shellType;
            newSession.workingPath = PathVariable.ReplacePath(preset.workingDirectory);
            Create(newSession);

            async void DelayInput()
            {
                while (IsCommandProcessing)
                {
                    await Task.Delay(100);
                }

                if (string.IsNullOrEmpty(preset.input)) return;

                await Input(preset.input);
            }

            TaskHelper.DoInmainThread(DelayInput);
        }

        private void Create(TerminalSessionInfo sessionInfo)
        {
            _sessionInfo = sessionInfo;
            _shell = Shell.Create(_sessionInfo.shellType);

            _process = new System.Diagnostics.Process();

            _process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.OutputDataReceived += OnOutputReceived;
            _process.ErrorDataReceived += OnErrorReceived;
            _process.StartInfo.FileName = _shell.GetExecutableName();
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.Environment.Add(ENV_KEY, "true");
            _process.StartInfo.Environment.Add("TERM", "xterm-mono");
            _process.Start();

            InputAsync($"cd {_sessionInfo.workingPath}", true)
                .ContinueWith(async (s) =>
                {
                    foreach (var command in _shell.GetBootCommands())
                    {
                        await InputAsync(command, true);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        public void Stop()
        {
            _process.Kill();
        }

        public TerminalSessionInfo GetSessionInfo()
        {
            return _sessionInfo;
        }

        public bool IsProcessing()
        {
            return IsCommandProcessing;
        }

        public async Task<string> Input(string command)
        {
            if (IsCommandProcessing) return "";
            var ret = await InputAsync(command);
            var workPathRet = await InputAsync(_shell.GetCurrentWorkpathCommand(), true);
            _sessionInfo.workingPath = workPathRet
                .Split('\n')
                .Select((x) => x.Trim())
                .Last(x => !string.IsNullOrEmpty(x));
            _sessionInfo.currentInput = "";

            return ret;
        }

        private Task<string> InputAsync(string command, bool hideOutput = false)
        {
            if (IsCommandProcessing) return Task.FromResult("");

            IsCommandProcessing = true;
            try
            {
                _sessionInfo.currentInputId += 1;
                if (hideOutput)
                {
                    _hiddenOutputInputIds.Add(_sessionInfo.currentInputId);
                }
                else
                {
                    _hiddenOutputInputIds.Clear();
                }

                _process.StandardInput.WriteLine(command);
                _process.StandardInput.Flush();
                _process.StandardInput.WriteLine($"echo {COMMAND_END_TOKEN}");
                _process.StandardInput.Flush();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Commander.Input error {e}");
                return Task.FromResult("");
            }

            var promise = new TaskCompletionSource<string>();
            this._onCommandCompleted = () =>
            {
                var outs = "";
                _sessionInfo.outputBlocks.ForEach((x) =>
                {
                    if (x.intputId == this._sessionInfo.currentInputId)
                    {
                        outs += x.line;
                    }
                });
                promise.TrySetResult(outs);
            };
            return promise.Task;
        }

        private void OnOutputReceived(object sender, DataReceivedEventArgs e)
        {
            AddMessage(StreamType.STDOUT, $"{e.Data}\n");
        }

        private void OnErrorReceived(object sender, DataReceivedEventArgs e)
        {
            AddMessage(StreamType.STDERR, $"{e.Data}\n");
        }

        private void AddMessage(string streamType, string message)
        {
            var messageWithoutTag = _parser.Parse(message, false);
            message = _parser.Parse(message, true);

            TaskHelper.DoInmainThread(() =>
            {
                bool isCompletedMessage = false;
                using (new DisposeGuard(null,
                    () =>
                    {
                        OnCommandProcessed?.Invoke();
                        if (isCompletedMessage)
                        {
                            IsCommandProcessing = false;
                            _onCommandCompleted?.Invoke();
                        }
                    }))

                {
                    if (string.IsNullOrEmpty(messageWithoutTag.Trim())) return;
                    if (messageWithoutTag.StartsWith(COMMAND_END_TOKEN))
                    {
                        // real end
                        isCompletedMessage = true;
                        return;
                    }

                    if (messageWithoutTag.Contains(COMMAND_END_TOKEN))
                    {
                        // echo command input output
                        return;
                    }

                    if (_hiddenOutputInputIds.Contains(_sessionInfo.currentInputId))
                    {
                        _sessionInfo.outputBlocks.Add(new TerminalStreamBlock
                        {
                            intputId = _sessionInfo.currentInputId,
                            type = StreamType.HIDDEN,
                            line = messageWithoutTag,
                            lineWithoutTag = messageWithoutTag
                        });
                        return;
                    }

                    if (0 == _sessionInfo.outputBlocks.Count)
                    {
                        _sessionInfo.outputBlocks.Add(new TerminalStreamBlock
                        {
                            intputId = _sessionInfo.currentInputId,
                            type = streamType,
                            line = message,
                            lineWithoutTag = messageWithoutTag
                        });
                        return;
                    }

                    var lastBlock = _sessionInfo.outputBlocks.Last();
                    if (lastBlock.intputId != _sessionInfo.currentInputId)
                    {
                        _sessionInfo.outputBlocks.Add(new TerminalStreamBlock
                        {
                            intputId = _sessionInfo.currentInputId,
                            type = streamType,
                            line = message,
                            lineWithoutTag = messageWithoutTag
                        });
                        return;
                    }

                    if (lastBlock.type != streamType)
                    {
                        _sessionInfo.outputBlocks.Add(new TerminalStreamBlock
                        {
                            intputId = _sessionInfo.currentInputId,
                            type = streamType,
                            line = message,
                            lineWithoutTag = messageWithoutTag
                        });
                        return;
                    }

                    lastBlock.line += message;
                    lastBlock.lineWithoutTag += messageWithoutTag;
                }
            });
        }
    }
}
#endif