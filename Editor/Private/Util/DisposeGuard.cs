#if UNITY_EDITOR
using System;

namespace SimpleAndLazy.Editor
{
    internal class DisposeGuard : IDisposable
    {
        private Action onEnd;

        public DisposeGuard(Action onStart, Action onEnd)
        {
            onStart?.Invoke();
            this.onEnd = onEnd;
        }

        public void Dispose()
        {
            this.onEnd?.Invoke();
        }
    }
}
#endif