#region Using

using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

#endregion

[InitializeOnLoad]
// ReSharper disable once CheckNamespace
public class VersionIncrement : IPreprocessBuildWithReport
{
    [MenuItem("Build/Build Current")]
    public static void BuildCurrent()
    {
        SetVersionFromTime();
        BuildPlayerWindow.ShowBuildPlayerWindow();
    }

    private static void IncrementVersion(int majorIncr, int minorIncr, int maintenanceIncr)
    {
        var currentVersion = PlayerSettings.bundleVersion;
        var major = Convert.ToInt32(currentVersion.Split('.')[0]) + majorIncr;
        var minor = Convert.ToInt32(currentVersion.Split('.')[1]) + minorIncr;
        var maintenance = Convert.ToInt32(currentVersion.Split('.')[2]) + maintenanceIncr;

        PlayerSettings.bundleVersion = major + "." + minor + "." + maintenance;
    }

    private static void SetVersion(int buildVersion)
    {
        PlayerSettings.iOS.buildNumber = "" + buildVersion + "";
        PlayerSettings.macOS.buildNumber = buildVersion.ToString();
        PlayerSettings.VisionOS.buildNumber = buildVersion.ToString();
        PlayerSettings.tvOS.buildNumber = buildVersion.ToString();

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
        Debug.LogWarning($"GTAP BUILD {major}.{minor}.{maintenance}.{buildVersion}");
    }

    public static string GetLocalVersion()
    {
        return PlayerSettings.bundleVersion;
    }

    [MenuItem("Build/Increase Minor Version")]
    private static void IncreaseMinor()
    {
        IncrementVersion(0, 1, 0);
    }

    [MenuItem("Build/Increase Major Version")]
    private static void IncreaseMajor()
    {
        IncrementVersion(1, 0, 0);
    }

    [MenuItem("Build/Increase Current Maintenance Version")]
    private static void IncreaseMaintenance()
    {
        IncrementVersion(0, 0, 1);
    }

    #region IPreprocessBuildWithReport Members

    public int callbackOrder => 1;

    public void OnPreprocessBuild(BuildReport report)
    {
        SetVersionFromTime();
    }

    private static void SetVersionFromTime()
    {
        Debug.LogWarning("GTAP BUILD OnPreprocessBuild auto increment");
        var a = new DateTime(2023, 07, 01, 00, 00, 00);
        var b = DateTime.UtcNow;
        SetVersion((int)b.Subtract(a).TotalHours);
    }

    #endregion
}