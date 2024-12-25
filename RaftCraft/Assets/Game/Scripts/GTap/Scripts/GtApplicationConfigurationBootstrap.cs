using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.CrashReportHandler;
using UnityEngine.Scripting;

namespace GTap.Scripts
{
    [Preserve]
    public sealed class GtApplicationConfigurationBootstrap
    {
        [Preserve]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void SubsystemRegistration()
        {
            Application.quitting += ApplicationOnQuitting;
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            Debug.Log("Start   " + AnalyticsSessionInfo.userId + " " + AnalyticsSessionInfo.sessionState + " " +
                      AnalyticsSessionInfo.sessionId + " " + AnalyticsSessionInfo.sessionElapsedTime);
            AnalyticsSessionInfo.sessionStateChanged += OnSessionStateChanged;
            AnalyticsSessionInfo.identityTokenChanged += AnalyticsSessionInfoOnidentityTokenChanged;
#endif
            ShowUsingSettings("SubsystemRegistration");
            Setup();
        }

        private static void ApplicationOnQuitting()
        {
            Application.quitting -= ApplicationOnQuitting;
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            AnalyticsSessionInfo.sessionStateChanged -= OnSessionStateChanged;
            AnalyticsSessionInfo.identityTokenChanged -= AnalyticsSessionInfoOnidentityTokenChanged;
#endif
        }

        private static void Setup()
        {
            SetBestSpeedForLoadingGame();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void BeforeSplashScreen()
        {
            DisableAnalytics();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void BeforeSceneLoad()
        {
            DisableAnalytics();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AfterSceneLoad()
        {
            SetBestSpeedForPlayGame();
            DisableAnalytics();
        }

        private static void SetBestSpeedForLoadingGame()
        {
            Application.backgroundLoadingPriority = ThreadPriority.High;
            Application.runInBackground = true;
            QualitySettings.maxQueuedFrames = 1;
            DisableAnalytics();
            ShowUsingSettings("SetBestSpeedForLoadingGame");
        }

        private static void SetBestSpeedForPlayGame()
        {
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            Application.runInBackground = false;
            QualitySettings.maxQueuedFrames = -1;
            ShowUsingSettings("SetBestSpeedForPlayGame");
        }

        private static void DisableAnalytics()
        {
//             try
//             {
//                 CrashReportHandler.enableCaptureExceptions = false;
// #if ENABLE_CLOUD_SERVICES_ANALYTICS
//                 Analytics.enabled = false;
//                 Analytics.deviceStatsEnabled = false;
//                 Analytics.initializeOnStartup = false;
//                 Analytics.limitUserTracking = true;
// #endif
//                 PerformanceReporting.enabled = false;
//
//                 var unityConnectModule = GetUnityConnectAssembly();
//                 SetInternalProperty(unityConnectModule, "UnityEngine.Connect.UnityConnectSettings", "enabled", false);
//                 SetInternalProperty(unityConnectModule, "UnityEngine.Advertisements.UnityAdsSettings", "enabled",
//                     false);
//             }
//             catch (Exception ex)
//             {
//                 Debug.LogError("Could not disable player tracking.");
//                 Debug.LogException(ex);
//             }
        }

        private static Assembly GetUnityConnectAssembly()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.GetName().Name == "UnityEngine.UnityConnectModule")
                {
                    return assembly;
                }
            }

            throw new Exception("Could not find the UnityConnectModule Assembly.");
        }

        private static void SetInternalProperty(Assembly assembly, string className, string propertyName, object value)
        {
            try
            {
                var type = assembly.GetType(className, true);
                var prop = type.GetProperty(propertyName,
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null)
                {
                    prop.SetValue(null, value, null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Could not set property '" + propertyName + "' of class '" + className + "'");
                Debug.LogException(ex);
            }
        }


        private static void ShowUsingSettings(string message)
        {
            if (Application.isEditor || Debug.isDebugBuild)
            {
                Debug.Log($"{message}:" +
#if ENABLE_CLOUD_SERVICES_ANALYTICS
                          $"\nAnalytics.enabled = {Analytics.enabled}," +
                          $"\nAnalytics.deviceStatsEnabled = {Analytics.deviceStatsEnabled}, " +
#endif
                          $"\nApplication.targetFrameRate = {Application.targetFrameRate}," +
                          $"\nApplication.backgroundLoadingPriority = {Application.backgroundLoadingPriority}," +
                          $"\nApplication.runInBackground = {Application.runInBackground}," +
                          $"\nQualitySettings.vSyncCount = {QualitySettings.vSyncCount}, " +
                          $"\nQualitySettings.maxQueuedFrames = {QualitySettings.maxQueuedFrames}");
            }
        }
#if ENABLE_CLOUD_SERVICES_ANALYTICS
        private static void OnSessionStateChanged(AnalyticsSessionState sessionState, long sessionId,
            long sessionElapsedTime, bool sessionChanged)
        {
            Debug.Log($"Call userId:{AnalyticsSessionInfo.userId}" +
                      $"\nsessionState:{sessionState}" +
                      $"\nsessionId:{sessionId}" +
                      $"\nsessionElapsedTime:{sessionElapsedTime}" +
                      "\nsessionChanged" +
#if ENABLE_CLOUD_SERVICES_ANALYTICS
                      $"\nAnalytics.enabled = {Analytics.enabled}," +
                      $"\nAnalytics.deviceStatsEnabled = {Analytics.deviceStatsEnabled}, " +
#endif
                      $"\nApplication.targetFrameRate = {Application.targetFrameRate}," +
                      $"\nApplication.backgroundLoadingPriority = {Application.backgroundLoadingPriority}," +
                      $"\nApplication.runInBackground = {Application.runInBackground}," +
                      $"\nQualitySettings.vSyncCount = {QualitySettings.vSyncCount}, " +
                      $"\nQualitySettings.maxQueuedFrames = {QualitySettings.maxQueuedFrames}");

            Debug.Log($"Call userId:{AnalyticsSessionInfo.userId}" +
                      $"\nsessionState:{sessionState}" +
                      $"\nsessionId:{sessionId}" +
                      $"\nsessionElapsedTime:{sessionElapsedTime}" +
                      "\nsessionChanged");
        }

        private static void AnalyticsSessionInfoOnidentityTokenChanged(string token)
        {
            Debug.Log($"Call userId:{AnalyticsSessionInfo.userId}" +
                      $"\ntoken:{token}");
        }
#endif
    }
}