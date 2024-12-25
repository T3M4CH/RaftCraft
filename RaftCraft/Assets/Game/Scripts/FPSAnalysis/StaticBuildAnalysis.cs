using UnityEngine;
using System;

public static class StaticBuildAnalysis
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Initialize()
    {
        AfterSceneLoadDateTime = DateTime.UtcNow;
        TimeSinceStartup = Time.realtimeSinceStartup;

        BundleVersion = Application.version;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void SubsystemRegistration()
    {
        var v = Time.realtimeSinceStartupAsDouble;
        ApplicationStartDateTime = DateTime.UtcNow;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void AfterAssembliesLoaded()
    {
        AfterAssembliesLoadedDateTime = DateTime.UtcNow;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void BeforeSplashScreen()
    {
        BeforeSplashScreenDateTime = DateTime.UtcNow;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BeforeSceneLoad()
    {
        BeforeSceneLoadDateTime = DateTime.UtcNow;
    }

    public static string BundleVersion { get; private set; }
    public static double TimeSinceStartup { get; private set; }

    public static DateTime ApplicationStartDateTime { get; private set; }
    public static DateTime AfterSceneLoadDateTime { get; private set; }
    public static DateTime BeforeSceneLoadDateTime { get; private set; }
    public static DateTime AfterAssembliesLoadedDateTime { get; private set; }
    public static DateTime BeforeSplashScreenDateTime { get; private set; }
}