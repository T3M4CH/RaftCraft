using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace GTap.Editor
{
    public class SplashScreenDisable : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; } = 99999;

        public void OnPreprocessBuild(BuildReport report)
        {
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SplashScreen.showUnityLogo = false;
        }
    }
}