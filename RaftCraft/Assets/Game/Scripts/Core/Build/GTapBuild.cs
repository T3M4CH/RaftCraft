#if UNITY_EDITOR
using System;
using Game.Scripts.Core.Debug;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace Plugins.GTapCore.Runtime.Scripts.Build
{
    public class GTapBuild : OdinEditorWindow
    {
        [MenuItem("GTapCore/Build")]
        private static void OpenWindow()
        {
            var window = GetWindow<GTapBuild>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(300, 300);
        }

        [ShowInInspector, InfoBox("Current Bundle Version Code Android")]
        private int BundleVersionAndroidCode => PlayerSettings.Android.bundleVersionCode;

        [ShowInInspector, InfoBox("Current Bundle Version Android")]
        private string BundleVersionAndroid => PlayerSettings.bundleVersion;
        
        [Button("Up Version Build From Time")]
        private void ButtonUpVersion()
        {
            SetVersionFromTime();
        }
        
        private void SetVersion(int buildVersion)
        {
            PlayerSettings.iOS.buildNumber = "" + buildVersion + "";
            PlayerSettings.macOS.buildNumber = buildVersion.ToString();

            if (buildVersion >= 90000)
            {
                throw new Exception(
                    "GTAP MAX BUILD https://docs.unity3d.com/ScriptReference/PlayerSettings.Android-bundleVersionCode.html");
            }

            PlayerSettings.Android.bundleVersionCode = buildVersion;
            var currentVersion = PlayerSettings.bundleVersion;
            var major = Convert.ToInt32(currentVersion.Split('.')[0]);
            var minor = Convert.ToInt32(currentVersion.Split('.')[1]);
            var maintenance = Convert.ToInt32(currentVersion.Split('.')[2]);
            CoreDebug.LogWarning($"GTAP BUILD {major}.{minor}.{maintenance}.{buildVersion}");
        }
        
        private void SetVersionFromTime()
        {
            CoreDebug.LogWarning("GTAP BUILD OnPreprocessBuild auto increment");
            var a = new DateTime(2023, 07, 01, 00, 00, 00);
            var b = DateTime.UtcNow;
            SetVersion((int)b.Subtract(a).TotalHours);
        }
        
        private void IncrementVersion(int majorIncr, int minorIncr, int maintenanceIncr)
        {
            var currentVersion = PlayerSettings.bundleVersion;
            var major = Convert.ToInt32(currentVersion.Split('.')[0]) + majorIncr;
            var minor = Convert.ToInt32(currentVersion.Split('.')[1]) + minorIncr;
            var maintenance = Convert.ToInt32(currentVersion.Split('.')[2]) + maintenanceIncr;

            PlayerSettings.bundleVersion = major + "." + minor + "." + maintenance;
        }
        
        [Button, ButtonGroup("Increase")]
        private void IncreaseMajor()
        {
            IncrementVersion(1, 0, 0);
        }
        
        [Button, ButtonGroup("Increase")]
        private void IncreaseMinor()
        {
            IncrementVersion(0, 1, 0);
        }
        
        [Button, ButtonGroup("Increase")]
        private void IncreaseMaintenance()
        {
            IncrementVersion(0, 0, 1);
        }

        [Button]
        private void Build()
        {
            BuildPlayerWindow.ShowBuildPlayerWindow();
        }
    }
}
#endif
