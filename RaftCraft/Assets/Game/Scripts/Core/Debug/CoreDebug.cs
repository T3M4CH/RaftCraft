namespace Game.Scripts.Core.Debug
{
   public class CoreDebug
   {
      public static void Log(string message)
      {
         if (GTapCoreSettings.Instance.Debug.HasFlag(DebugMode.Log))
         {
            UnityEngine.Debug.Log($"[GTapDebug]: {message}");
         }
      }

      public static void LogWarning(string message)
      {
         if (GTapCoreSettings.Instance.Debug.HasFlag(DebugMode.LogWarning))
         {
            UnityEngine.Debug.LogWarning($"[GTapDebug]: {message}");
         }
      }

      public static void LogError(string message)
      {
         if (GTapCoreSettings.Instance.Debug.HasFlag(DebugMode.LogError))
         {
            UnityEngine.Debug.LogError($"[GTapDebug]: {message}");
         }
      }
   }
}
