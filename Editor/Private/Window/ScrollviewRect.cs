#if UNITY_EDITOR
using System.Net;
using UnityEditor;
using UnityEngine;

namespace SimpleAndLazy.Editor
{
    internal class ScrollviewRect
    {
        private Rect _rect = new Rect();

        public ScrollviewRect()
        {
        }

        public void AddLastRectHeight()
        {
            if (Event.current.type != EventType.Repaint) return;
            var rect = GUILayoutUtility.GetLastRect();
            _rect.height += rect.height + 2.0f;
        }

        public void AddLastRectWidth()
        {
            if (Event.current.type != EventType.Repaint) return;
            var rect = GUILayoutUtility.GetLastRect();
            _rect.width += rect.width;
        }

        public Rect? End()
        {
            if (Event.current.type == EventType.Repaint)
            {
                return _rect;
            }

            return null;
        }
    }
}
#endif