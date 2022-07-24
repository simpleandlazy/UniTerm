#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;

namespace SimpleAndLazy.Editor
{
    struct ParseReturn
    {
        public int pos;
        public Color? color;

        public static readonly ParseReturn Default = new ParseReturn {pos = 0, color = null};
    }

    internal class XtermParser
    {
        // https://en.wikipedia.org/wiki/ANSI_escape_code#ESC
        // https://en.wikipedia.org/wiki/C0_and_C1_control_codes
        // https://invisible-island.net/xterm/ctlseqs/ctlseqs.html#h2-Definitions
        private const byte LF = 0x0A;
        private const byte FF = 0x0C;
        private const byte CR = 0x0D;
        private const byte ESC = 0x1B;
        private const byte OpenSquareBracket = (byte) '[';
        private Color curForegroundColor = XtermColor.DefaultColor;

        public string Parse(string contents, bool isColorOn)
        {
            List<byte> converted = new List<byte>();
            AddColor(isColorOn, converted, curForegroundColor);
            contents = ReplaceREPLACEMENT_CHARACTER(contents);

            byte[] bytes = Encoding.UTF8.GetBytes(contents);
            int cursor = 0;
            while (cursor < bytes.Length)
            {
                byte curByte = bytes[cursor];
                byte? nextByte = null;
                if (cursor < bytes.Length - 1) nextByte = bytes[cursor + 1];
                if (curByte == ESC && null != nextByte && nextByte == OpenSquareBracket)
                {
                    var parseReturn = HandleControlSequenceIntroducer(bytes, cursor);
                    cursor = parseReturn.pos;
                    if (null != parseReturn.color)
                    {
                        CloseColor(isColorOn, converted);
                        curForegroundColor = parseReturn.color.Value;
                        AddColor(isColorOn, converted, curForegroundColor);
                    }

                    continue;
                }
                else
                {
                    converted.Add(bytes[cursor]);
                }

                cursor++;
            }

            CloseColor(isColorOn, converted);

            string ret = Encoding.UTF8.GetString(converted.ToArray());
            return ret;
        }

        private static ParseReturn HandleControlSequenceIntroducer(byte[] bytes, int cursor)
        {
            ParseReturn ret = ParseReturn.Default;
            int escapeCursor = cursor;

            cursor = cursor + 2; // Pass CSI token
            while (cursor < bytes.Length
                   && !(0x40 <= bytes[cursor] && bytes[cursor] < 0x7E)
            )
            {
                cursor++;
            }

            if (cursor == bytes.Length)
            {
                ret.pos = cursor;
                return ret;
            }

            var endMCursor = cursor;
            if (bytes[endMCursor] == 'm') return HandleSelectGraphicRendition(bytes, escapeCursor, endMCursor);
            ret.pos = endMCursor + 1;
            return ret;
        }

        private static ParseReturn HandleSelectGraphicRendition(byte[] bytes, int escapeCursor, int endMCursor)
        {
            ParseReturn ret = ParseReturn.Default;
            ret.pos = endMCursor + 1;

            List<byte> subBytes = new List<byte>();
            for (int i = escapeCursor + 1; i < endMCursor + 1; i++)
            {
                subBytes.Add(bytes[i]);
            }

            string str = Encoding.UTF8.GetString(subBytes.ToArray());
            var color = XtermColor.Handle_3or4Bit_ForegroundColor(str);
            if (null != color)
            {
                ret.color = color;
                return ret;
            }

            color = XtermColor.Handle_8Bit_ForegroundColor(str);
            if (null != color)
            {
                ret.color = color;
                return ret;
            }

            if (!str.StartsWith("[38;2;"))
            {
                return ret;
            }

            Regex rx = new Regex(@"38;2;(?<red>[0-9]+);(?<green>[0-9]+);(?<blue>[0-9]+)m",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var match = rx.Match(str);
            if (!match.Success)
            {
                return ret;
            }

            var r = int.Parse(match.Groups["red"].ToString()) / (float) 255;
            var g = int.Parse(match.Groups["green"].ToString()) / (float) 255;
            var b = int.Parse(match.Groups["blue"].ToString()) / (float) 255;
            ret.color = new Color(r, g, b);
            return ret;
        }


        // https://en.wikipedia.org/wiki/Specials_(Unicode_block)
        private static string ReplaceREPLACEMENT_CHARACTER(string contents)
        {
            return contents.Replace("�", "");
        }

        private static void AddColor(bool isColorOn, List<byte> converted, Color color)
        {
            if (!isColorOn) return;
            var bytes = Encoding.UTF8.GetBytes(color.ToRichTextTag());
            converted.AddRange(bytes);
        }

        private static void CloseColor(bool isColorOn, List<byte> converted)
        {
            if (!isColorOn) return;
            var bytes = Encoding.UTF8.GetBytes("</color>");
            converted.AddRange(bytes);
        }
    }
}
#endif