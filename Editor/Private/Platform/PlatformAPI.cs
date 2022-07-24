#if UNITY_EDITOR
namespace SimpleAndLazy.Editor
{
    internal class PlatformAPI
    {
        private static PlatformAPIBase instance;

        static PlatformAPI()
        {
            switch (EditorPlatform.Get())
            {
                case EditorPlatform.Type.Windows:
                    instance = new WindowsPlatformAPI();
                    break;
                case EditorPlatform.Type.Mac:
                case EditorPlatform.Type.Linux:
                    instance = new UnixPlatformAPI();
                    break;
            }
        }

        public static PlatformAPIBase Get()
        {
            return instance;
        }
    }
}
#endif