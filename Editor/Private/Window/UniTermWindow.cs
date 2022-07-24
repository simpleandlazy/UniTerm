#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using SimpleAndLazy.Editor.Public;
using UnityEditor;
using UnityEngine;

namespace SimpleAndLazy.Editor
{
    class UniTermWindow : EditorWindow, IUniTermWindow
    {
        private static UniTermWindow instance;

        // Controller
        private TerminalController _controller;
        private CommandPresetController _presetController;

        // Shell Topbar
        private int shellPopupIndex = 0;

        // Preset Topbar
        private bool presetFoldout = false;
        private List<CommandPresetUIInfo> presetUiInfos = new List<CommandPresetUIInfo>();

        // TerminalNames Tab
        private List<TerminalNamesUIInfo> terminalNamesUIInfos = new List<TerminalNamesUIInfo>();
        private Vector2 terminalNameScrollPos;
        private int terminalTabIndex = 0;
        private bool refocusTerminalNamesTab = false;

        // TerminalInput
        private bool refocusInput = false;
        private bool befHasFocus = false;

        // TerminalOutput
        private Rect scrollViewContentRect = new Rect();
        private Vector2 terminalOutputScrollPos;

        // Etc
        private static int onguiCount = 0;
        private Tuple<Vector2, Action> onPosClick;
        private Vector2 thisFrameGlobalMousePosition;

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            SimpleAndLazy.Editor.Public.UniTermWindow.GetInstance = () =>
            {
                if (null == instance)
                {
                    instance = CreateWindow();
                }

                return instance;
            };
        }


        [MenuItem("Window/UniTerm")]
        public static void ShowWindow()
        {
            CreateWindow();
        }

        private static UniTermWindow CreateWindow()
        {
            var window = EditorWindow.GetWindow<UniTermWindow>(false, "UniTerm", true);
            window.minSize = new Vector2(400, 300);
            window.wantsMouseMove = true;
            window.InitializeController();
            return window;
        }


        private void OnLostFocus()
        {
            befHasFocus = false;
        }

        private void OnTabChanged()
        {
            RefreshFocus();
        }

        void OnGUI()
        {
            ++onguiCount;
            thisFrameGlobalMousePosition = Event.current.mousePosition;

            InitializeController();
            CheckPosClick();
            CheckShortcut();

            if (!befHasFocus && hasFocus)
            {
                befHasFocus = true;
                TaskHelper.DoInmainThread(RefreshFocus);
            }

            try
            {
                OnGUIShellTopBar();

                OnGUIPreset();

                if (0 == _controller.Terminals.Count()) return;

                OnGUITerminalNames();

                OnGUITerminal(_controller.Terminals[terminalTabIndex]);
            }
            catch (Exception e)
            {
                // Debug.LogError($"UTermWindow.OnGUI exception:{e}");
                if (GUILayout.Button(new GUIContent("DeleteAll Sessions",
                    $"Exception {e.Message} occured. \n Trace:{e.StackTrace}")))
                {
                    TerminalController.RemoveAll();
                }
            }

            // DebugMousePosition();
        }


        #region GUI

        private void OnGUIShellTopBar()
        {
            using (var _ = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("ShellType", WindowStyles.Style_TopbarLabel, WindowStyles.Layouts_TopbarLabel);
                shellPopupIndex = EditorGUILayout.Popup(shellPopupIndex,
                    PlatformAPI.Get().GetAvailableShellTypes().Select((x) => x.ToAlias()).ToArray(),
                    WindowStyles.Layouts_Dropdown);

                if (GUILayout.Button(WindowResources.AddShellButtonContent, WindowStyles.Layouts_SmallButton))
                {
                    AddTerminal();
                }

                if (GUILayout.Button(WindowResources.BinShellButtonContent, WindowStyles.Layouts_SmallButton))
                {
                    if (0 < _controller.Terminals.Count())
                    {
                        ClearOutput(_controller.Terminals[terminalTabIndex]);
                    }
                }
            }
        }

        private void OnGUIPreset()
        {
            presetFoldout = EditorGUILayout.Foldout(presetFoldout, "Preset");
            if (presetFoldout)
            {
                presetUiInfos.Resize(_presetController.Presets.Count);
                for (int presetIndex = 0; presetIndex < _presetController.Presets.Count; presetIndex++)
                {
                    var preset = _presetController.Presets[presetIndex];
                    var presetUiInfo = presetUiInfos[presetIndex];
                    using (var _ = new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Space(10.0f);
                        presetUiInfo.foldOut = EditorGUILayout.Foldout(presetUiInfo.foldOut, preset.name);


                        if (GUILayout.Button(WindowResources.RunPresetButtonContent,
                            WindowStyles.Layouts_SmallButton))
                        {
                            AddTerminalPreset(preset);
                        }


                        if (GUILayout.Button(WindowResources.ClosePresetButtonContent,
                            WindowStyles.Layouts_SmallButton))
                        {
                            if (0 < _presetController.Presets.Count())
                            {
                                _presetController.RemovePreset(_presetController.Presets[presetIndex]);
                            }
                        }
                    }

                    if (presetUiInfo.foldOut)
                    {
                        using (var __ = new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Space(20.0f);
                            GUILayout.Label("Name", WindowStyles.Layouts_TopbarLabel);
                            preset.name = GUILayout.TextField(preset.name, 50);
                        }

                        using (var __ = new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Space(20.0f);
                            GUILayout.Label("ShellType", WindowStyles.Layouts_TopbarLabel);
                            presetUiInfo.shellTypePopupIndex = EditorGUILayout.Popup(
                                presetUiInfo.shellTypePopupIndex,
                                PlatformAPI.Get().GetAvailableShellTypes().Select((x) => x.ToAlias()).ToArray(),
                                WindowStyles.Layouts_Dropdown);
                            preset.shellType =
                                PlatformAPI.Get().GetAvailableShellTypes()[presetUiInfo.shellTypePopupIndex];
                        }

                        using (var __ = new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Space(20.0f);
                            GUILayout.Label("WorkingPath", WindowStyles.Layouts_TopbarLabel);
                            preset.workingDirectory = GUILayout.TextField(preset.workingDirectory, 200);
                        }

                        using (var __ = new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Space(20.0f);
                            GUILayout.Label("Command", WindowStyles.Layouts_TopbarLabel);
                            preset.input = GUILayout.TextField(preset.input, 50);
                        }

                        using (var __ = new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Space(20.0f);
                            if (GUILayout.Button(WindowResources.CheckPresetButtonContent,
                                WindowStyles.Layouts_SmallButton))
                            {
                                _presetController.Save();
                            }
                        }
                    }
                }

                using (var _ = new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(10.0f);
                    if (GUILayout.Button(WindowResources.AddPresetButtonContent,
                        WindowStyles.Layouts_SmallButton))
                    {
                        var newPreset = new CommandPreset
                        {
                            name = "New preset",
                            shellType = GetSelectedShellType(),
                            workingDirectory = "${projectPath}/",
                            input = ""
                        };
                        _presetController.AddPreset(newPreset);
                    }
                }
            }
        }

        private void OnGUITerminalNames()
        {
            terminalNamesUIInfos.Resize(_controller.Terminals.Count);
            if (refocusTerminalNamesTab)
            {
                var targetPos = terminalNamesUIInfos[terminalTabIndex].rect.position;
                if (targetPos == Vector2.zero && 1 < terminalTabIndex)
                {
                    var beftargetPos = terminalNamesUIInfos[terminalTabIndex - 1].rect.position +
                                       terminalNamesUIInfos[terminalTabIndex - 1].rect.size;
                    targetPos = beftargetPos;
                }

                terminalNameScrollPos = targetPos;
                refocusTerminalNamesTab = false;
            }

            using (var scrollView =
                new EditorGUILayout.ScrollViewScope(terminalNameScrollPos,
                    false,
                    false,
                    WindowStyles.Style_TerminalNamesScrollbar,
                    GUIStyle.none,
                    GUIStyle.none,
                    WindowStyles.Layouts_TerminalNameScrollView))
            {
                terminalNameScrollPos = scrollView.scrollPosition;

                using (var _ = new EditorGUILayout.HorizontalScope())
                {
                    var befTabIndex = terminalTabIndex;
                    for (int i = 0; i < _controller.Terminals.Count; i++)
                    {
                        var term = _controller.Terminals[i];
                        var tname = term.Name;
                        var style = i == befTabIndex
                            ? WindowStyles.Style_SelectedButton
                            : WindowStyles.Style_NonSelectedButton;
                        if (GUILayout.Button(tname, style))
                        {
                            terminalTabIndex = i;
                        }

                        if (Event.current.type == EventType.Repaint)
                            terminalNamesUIInfos[i].rect = GUILayoutUtility.GetLastRect();

                        OnHoverRect(GUILayoutUtility.GetLastRect(), rect =>
                        {
                            var size = 12.0f;
                            rect.x = rect.x + rect.width - size - 3.0f;
                            rect.y = rect.y + (rect.height - size) / 2;
                            rect.width = size;
                            rect.height = size;
                            GUI.Label(rect, WindowResources.CloseShellButtonContent);
                            OnClickRect(rect, () => { TaskHelper.DoInmainThread(() => RemoveTerminal(term)); });
                        });
                    }

                    terminalTabIndex = Math.Max(0, Math.Min(terminalTabIndex, _controller.Terminals.Count() - 1));
                    if (befTabIndex != terminalTabIndex) TaskHelper.DoInmainThread(OnTabChanged);
                }
            }
        }


        void OnGUITerminal(Terminal terminal)
        {
            var befBackground = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f);
            {
                var lastRect = GUILayoutUtility.GetLastRect();
                var curPos = new Vector2(lastRect.x, lastRect.y + lastRect.height);
                GUI.Box(new Rect(
                    curPos.x + scrollViewContentRect.x,
                    curPos.y + scrollViewContentRect.y,
                    Screen.width,
                    scrollViewContentRect.height + 3
                ), "");
            }

            GUI.backgroundColor = Color.clear;
            using (var scrollView =
                new EditorGUILayout.ScrollViewScope(terminalOutputScrollPos, WindowStyles.Layouts_TerminalScrollView))
            {
                terminalOutputScrollPos = scrollView.scrollPosition;

                var sr = new ScrollviewRect();
                OnGUITerminalOutputBlocks(terminal, sr);
                OnGUITerminalInputField(terminal, sr);
                Rect? newRect = sr.End();
                if (null != newRect)
                {
                    var befRect = scrollViewContentRect;
                    scrollViewContentRect = newRect.Value;
                    if (befRect != scrollViewContentRect)
                    {
                        Repaint();
                    }
                }
            }

            GUI.backgroundColor = befBackground;
        }

        void OnGUITerminalOutputBlocks(Terminal terminal, ScrollviewRect scrollviewRect)
        {
            foreach (var block in terminal.OutputBlocks)
            {
                if (block.type == StreamType.HIDDEN) continue;
                var contents = block.lineWithoutTag.Trim();
                var contentsWithColor = block.line.Trim();

                GUILayout.TextArea(contentsWithColor, WindowStyles.Style_TerminalOutput,
                    WindowStyles.TerminalOutputSize);

                scrollviewRect.AddLastRectHeight();
                OnHoverRect(GUILayoutUtility.GetLastRect(), rect =>
                {
                    var size = 20.0f;
                    rect.x = rect.x + rect.width - size - 3.0f;
                    rect.y = rect.y + 3.0f;
                    rect.width = size;
                    rect.height = size;
                    GUI.Label(rect, WindowResources.ClipboardButtonContent,
                        WindowStyles.Style_TerminalOutputClipboardLabel);
                    OnClickRect(rect, () => { Etc.CopyToClipboard(contents); });
                });
            }
        }

        async void OnGUITerminalInputField(Terminal terminal, ScrollviewRect scrollviewRect)
        {
            var afterText = "";
            using (var _ = new EditorGUILayout.HorizontalScope())
            {
                if (terminal.IsCommandProcessing)
                {
                    GUILayout.Label("...", new[] {GUILayout.ExpandWidth(false)});
                    return;
                }

                var prefix = $"{terminal.WorkingPathShort}$ ";
                GUILayout.Label(prefix, new[] {GUILayout.ExpandWidth(false)});
                GUI.SetNextControlName("TerminalInput");
                afterText = GUILayout.TextArea(terminal.GetSessionInfo().currentInput,
                    WindowStyles.Style_TerminalInput,
                    WindowStyles.LayOuts_TerminalInput);
            }

            scrollviewRect.AddLastRectHeight();

            if (refocusInput)
            {
                EditorGUI.FocusTextInControl("TerminalInput");
                refocusInput = false;
            }


            {
                var newInput = afterText.Replace("\n", "");
                terminal.GetSessionInfo().currentInput = newInput;

                if (Event.current.type == EventType.KeyUp && Event.current.keyCode == (KeyCode.Return))
                {
                    UnFocus();

                    await terminal.Input(newInput);
                    TaskHelper.DoInmainThread(RefreshFocus);
                    refocusInput = true;
                }
            }
        }

        #endregion


        #region HandleGUI

        private void UnFocus()
        {
            GUI.SetNextControlName("Hidden");
            GUILayout.TextField("", WindowStyles.LayOuts_HIDDEN);
            EditorGUI.FocusTextInControl("Hidden");
        }

        private void RefreshFocus()
        {
            refocusInput = true;
            refocusTerminalNamesTab = true;
            terminalOutputScrollPos = new Vector2(0, scrollViewContentRect.height);
            Repaint();
        }


        private void CheckPosClick()
        {
            if (null != Event.current
                && null != onPosClick
                && Event.current.type == EventType.MouseUp
                && onPosClick.Item1 == Event.current.mousePosition)
            {
                onPosClick.Item2?.Invoke();
            }
        }

        private void CheckShortcut()
        {
            if (null == Event.current) return;
            if (Event.current.type != EventType.KeyUp) return;
            if (EditorPlatform.IsPlatformControlKeyPressed(Event.current))
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.T:
                    {
                        TaskHelper.DoInmainThread(AddTerminal);
                        break;
                    }
                    case KeyCode.W:
                    {
                        TaskHelper.DoInmainThread(() =>
                        {
                            if (0 < _controller.Terminals.Count())
                            {
                                var term = _controller.Terminals[terminalTabIndex];
                                RemoveTerminal(term);
                            }
                        });
                        break;
                    }
                    case KeyCode.L:
                    {
                        TaskHelper.DoInmainThread(() =>
                        {
                            if (0 < _controller.Terminals.Count())
                            {
                                ClearOutput(_controller.Terminals[terminalTabIndex]);
                                RefreshFocus();
                            }
                        });
                        break;
                    }
                }
            }
        }


        private static void DebugMousePosition()
        {
            var mousePosition = Event.current.mousePosition;
            float x = mousePosition.x;
            float y = mousePosition.y;
            float width = 200;
            float height = 30;
            var rect = new Rect(x, y, width, height);

            GUI.Label(rect, $"{Event.current.mousePosition}");
        }

        private void OnHoverRect(Rect rect, Action<Rect> action)
        {
            if (rect.Contains(Event.current.mousePosition))
            {
                action?.Invoke(new Rect(rect));
            }
        }

        private void OnClickRect(Rect rect, Action action)
        {
            if (rect.Contains(Event.current.mousePosition))
            {
                onPosClick = Tuple.Create<Vector2, Action>(thisFrameGlobalMousePosition,
                    action);
            }
        }

        #endregion


        #region Command

        private void InitializeController()
        {
            if (null == instance)
            {
                instance = this;
            }

            if (null == _controller)
            {
                _controller = new TerminalController();
                _controller.OnCommandProcessed += RefreshFocus;
            }

            if (null == _presetController)
            {
                _presetController = new CommandPresetController();
            }
        }

        internal ShellType GetSelectedShellType()
        {
            return PlatformAPI.Get().GetAvailableShellTypes()[shellPopupIndex];
        }

        private void AddTerminal()
        {
            _controller.AddTerminal(GetSelectedShellType());
            var index = _controller.Terminals.Count - 1;
            terminalTabIndex = index;
            refocusTerminalNamesTab = true;
        }

        public IUniTermInternal AddTerminalPreset(CommandPreset preset)
        {
            var ret = _controller.AddTerminal(preset);
            var index = _controller.Terminals.Count - 1;
            terminalTabIndex = index;
            refocusTerminalNamesTab = true;

            return ret;
        }

        private void RemoveTerminal(Terminal term)
        {
            _controller.RemoveTerminal(term);
            terminalTabIndex = Math.Max(0, Math.Min(terminalTabIndex, _controller.Terminals.Count() - 1));
        }

        private void ClearOutput(Terminal term)
        {
            _controller.ClearOutput(term);
        }

        #endregion
    }
}
#endif