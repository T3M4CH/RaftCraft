using System.Collections.Generic;

namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class CustomEvent : AnalyticsEvent
    {
        public override string EventName { get; }
        public Dictionary<string, object> Parameters { get; }

        public CustomEvent(string eventName, Dictionary<string, object> parameters) : base(HomaGames.HomaBelly.EventCategory.custom_event.ToString())
        {
            this.EventName = ToUnderscoreCase(eventName);
            Parameters = new Dictionary<string, object>();
            foreach (KeyValuePair<string,object> keyValuePair in parameters)
            {
                Parameters.Add(ToUnderscoreCase(keyValuePair.Key), keyValuePair.Value);
            }
        }

        public override Dictionary<string, object> GetData()
        {
            return Parameters;
        }
    }
}