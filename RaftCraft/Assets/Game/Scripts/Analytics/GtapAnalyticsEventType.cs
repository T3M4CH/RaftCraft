using System.Collections.Generic;

namespace GTap.Analytics
{
    public enum GtapAnalyticsEventType
    {
        Undefined = 0,
        Application = 1,
        Session = 2,
        Ad = 3,
        Level = 4,
        IAP = 5,
        Debug = 6,
        ML = 7,
        ABTest = 8,
        Heartbeat = 9,
        Funnel = 10,
        Error = 11,
        Custom = 12,
        Skin = 13,
        Tutorial = 14,
        Technical = 15,
    }

    public enum GtapAnalyticsAdErrorType
    {
        Undefined = 0,
        Unknown = 1,
        Offline = 2,
        NoFill = 3,
        InternalError = 4,
        InvalidRequest = 5,
        UnableToPrecache = 6
    }

    public enum GtapAnalyticsIAPEventStore
    {
        Undefined = 0,
        GooglePlay = 1,
        AppleAppStore = 2,
        Other = 3
    }

    public enum GtapAnalyticsIAPEventType
    {
        Undefined = 0,
        Purchase = 1,
        Trade = 2,
        Sell = 3,
        Return = 4
    }

    public struct GtapGameEvent
    {
        public GtapAnalyticsEventType GtapAnalyticsEventType;
        public GtapAnalytics.EventType EventType;
        public GtapAnalyticsIAPEventType GtapAnalyticsIAPEventType;
        public GtapAnalyticsIAPEventStore GtapAnalyticsIAPStore;

        public Dictionary<string, object> EventParams;

        public GtapGameEvent(GtapGameEvent e)
        {
            GtapAnalyticsEventType = e.GtapAnalyticsEventType;
            EventType = e.EventType;
            GtapAnalyticsIAPStore = e.GtapAnalyticsIAPStore;
            GtapAnalyticsIAPEventType = e.GtapAnalyticsIAPEventType;
            EventParams = e.EventParams;
        }

        public T TryGetParam<T>(string p)
        {
            if (EventParams.ContainsKey(p))
            {
                return (T)EventParams[p];
            }

            return default;
        }
    }

    public enum EResourceCollected
    {
        Fish,
        WaterResources
    }
    
    public class EventConstants
    {
        public static string Upgrade = nameof(Upgrade);
        public static string BuyNpc = nameof(BuyNpc);
        public static string BuyWeapon = nameof(BuyWeapon);
        public static string UnderwaterDeaths = nameof(UnderwaterDeaths);
        public static string TimeSpentInWater = nameof(TimeSpentInWater);
        public static string ResourceCollected = nameof(ResourceCollected);
        public static string EventTileBuild = "EventTileBuild";
    }
}