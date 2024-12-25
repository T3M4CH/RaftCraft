namespace HomaGames.HomaBelly
{
	 public static partial class HomaBridgeDependencies
	 {
	 	 static partial void PartialInstantiateOldAnalytics()
	 	 {
// Homa Belly services won't run on Unity Editor. This is a design decision to allow some runtime optimizations
#if !UNITY_EDITOR
			 // This method will be filled automatically by:HomaBridgeDependenciesCodeGenerator when Homa Belly services are added/removed to the project.
			 analytics.Add(new AnalyticsInterfaceForwarder(new FacebookImplementation()));
			 analytics.Add(new AnalyticsInterfaceForwarder(new GameAnalyticsImplementation()));
			 analytics.Add(new AnalyticsInterfaceForwarder(new HomaAnalyticsAdapter()));
#endif
	 	 }
	 }
}
