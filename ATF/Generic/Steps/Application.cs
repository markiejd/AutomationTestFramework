
using Core.Logging;

namespace Generic.Steps
{
    public static class Application
    {

        public static bool OpenApplication(string applicationExePath)
        {
            DebugOutput.Log($"proc - OpenApplication");
            return false;
        }

        public static void CloseWinDriver()
        {
            DebugOutput.Log($"proc - CloseWinDriver");
        }
    }
}
