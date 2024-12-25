using System;
using System.Text;
using HomaGames.HomaBelly.Internal.Analytics;
using UnityEngine;
using NetworkReachability = HomaGames.HomaBelly.Internal.Analytics.NetworkReachability;
#if HOMA_STORE
using HomaGames.HomaBelly.IAP;
#endif

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// Acts as a wrapper to allow old <see cref="IAnalytics"/> implementers
    /// to work with the new <see cref="AnalyticsBase"/> based system.
    /// </summary>
    public class AnalyticsInterfaceForwarder : AnalyticsBase
    {
        private readonly IAnalytics Implementer;
        
        private bool IsForwardingToHomaAnalytics { get; }

        public AnalyticsInterfaceForwarder(IAnalytics implementer)
        {
            Implementer = implementer;

            IsForwardingToHomaAnalytics = Implementer.GetType().Name.Contains("HomaAnalytics");
        }

        public override void Initialize(Action onInitialized = null)
        {
            if (Implementer is IAnalyticsWithInitializationCallback analyticsWithInitializationCallback)
            {
                analyticsWithInitializationCallback.Initialize(onInitialized);
            }
            else
            {
                Implementer.Initialize();
                
                onInitialized?.Invoke();
            }
        }

        public override void OnApplicationPause(bool pause) 
            => Implementer.OnApplicationPause(pause);

        public override void ValidateIntegration() 
            => Implementer.ValidateIntegration();

        public override void SetUserIsAboveRequiredAge(bool consent) 
            => Implementer.SetUserIsAboveRequiredAge(consent);

        public override void SetTermsAndConditionsAcceptance(bool consent) 
            => Implementer.SetTermsAndConditionsAcceptance(consent);

        public override void SetAnalyticsTrackingConsentGranted(bool consent) 
            => Implementer.SetAnalyticsTrackingConsentGranted(consent);

        public override void SetTailoredAdsConsentGranted(bool consent) 
            => Implementer.SetTailoredAdsConsentGranted(consent);
        
#if UNITY_PURCHASING
        public override void TrackInAppPurchaseEvent(UnityEngine.Purchasing.Product product, bool isRestored = false)
            => Implementer.TrackInAppPurchaseEvent(product, isRestored);
#endif

        public override void TrackErrorEvent(ErrorSeverity severity, string message) 
            => Implementer.TrackErrorEvent(severity, message);

        public override void TrackAdRevenue(AdRevenueData adRevenueData)
        {
            if (Implementer is IAnalyticsAdRevenue instance)
            {
                instance.TrackAdRevenue(adRevenueData);
            }
        }


        public override void SetCustomDimension01(string customDimension)
        {
            if (Implementer is ICustomDimensions instance)
            {
                instance.SetCustomDimension01(customDimension);
            }
        }

        public override void SetCustomDimension02(string customDimension)
        {
            if (Implementer is ICustomDimensions instance)
            {
                instance.SetCustomDimension02(customDimension);
            }
        }

        public override void SetCustomDimension03(string customDimension)
        {
            if (Implementer is ICustomDimensions instance)
            {
                instance.SetCustomDimension03(customDimension);
            }
        }

        public override void TrackEvent(AnalyticsEvent analyticsEvent)
        {
            switch (analyticsEvent.EventCategory)
            {
                case nameof(EventCategory.progression_event):
                    TranslateProgressionEventToLegacy(analyticsEvent);
                    break;
                case nameof(EventCategory.session_event):
                    TranslateSessionEventToLegacy(analyticsEvent);
                    break;
                case nameof(EventCategory.system_event):
                    TranslateSystemEventToLegacy(analyticsEvent);
                    break;
                case nameof(EventCategory.design_event):
                    TranslateDesignEventToLegacy(analyticsEvent);
                    break;
                case nameof(EventCategory.custom_event):
                    TranslateCustomEventToLegacy(analyticsEvent);
                    break;
                case nameof(EventCategory.ad_event):
                    TranslateAdEventToLegacy(analyticsEvent);
                    break;
                case nameof(EventCategory.iap_event):
                    TranslateIAPEventToLegacy(analyticsEvent);
                    break;
                case nameof(EventCategory.item_event):
                    TranslateItemEventToLegacy(analyticsEvent);
                    break;
                case nameof(EventCategory.resource_event):
                    TranslateResourceEventToLegacy(analyticsEvent);
                    break;
                case nameof(EventCategory.internal_package):
                    TranslateInternalPackageEventToLegacy(analyticsEvent);
                    break;
                default:
                    base.TrackEvent(analyticsEvent);
                    break;
            }
        }

        private void TranslateInternalPackageEventToLegacy(AnalyticsEvent analyticsEvent)
        {
            if (analyticsEvent is InternalPackage internalPackage)
            {
                Implementer.TrackDesignEvent($"InternalPackage:{internalPackage.PackageName}:{internalPackage.Version}:{internalPackage.Status}");
            }
        }

        private void TranslateResourceEventToLegacy(AnalyticsEvent analyticsEvent)
        {
            if (analyticsEvent is ResourceFlow resourceFlow)
            {
                Implementer.TrackResourceEvent(resourceFlow.FlowType, resourceFlow.Currency, resourceFlow.FlowAmount, null, null);
            }
        }

        private void TranslateItemEventToLegacy(AnalyticsEvent analyticsEvent)
        {
            if (analyticsEvent is ItemEvent itemEvent)
            {
                if (itemEvent is ItemConsumed itemConsumed)
                {
                    Implementer.TrackResourceEvent(ResourceFlowType.Sink, "item", 0f, null, itemConsumed.ItemId);
                }
                else if (itemEvent is ItemObtained itemObtained)
                {
                    Implementer.TrackResourceEvent(ResourceFlowType.Source, "item", 0f, null, itemObtained.ItemId);
                }
                else if (itemEvent is ItemUpgraded itemUpgraded)
                {
                    Implementer.TrackDesignEvent($"Item:Upgraded:{itemUpgraded.ItemId}", itemUpgraded.ItemLevel);
                }
            }
        }

        private void TranslateIAPEventToLegacy(AnalyticsEvent analyticsEvent)
        {
            if (analyticsEvent is IAPEvent iapEvent)
            {
                if (iapEvent is IAPProductEvent iapProductEvent)
                {
                    if (iapProductEvent is IAPPurchaseStarted iapPurchaseStarted)
                    {
                        Implementer.TrackDesignEvent($"IAP:PurchaseStarted:{iapPurchaseStarted.ProductId}");
                    }
                    else if (iapProductEvent is IAPPurchaseCompleted iapPurchaseCompleted)
                    {
                        #if HOMA_STORE
                        Product purchasedProduct = HomaStore.GetProduct(iapPurchaseCompleted.ProductId);
                        Implementer.TrackInAppPurchaseEvent(iapPurchaseCompleted.ProductId, purchasedProduct.CurrencyCode, purchasedProduct.Price);
                        #endif
                    }
                    else if (iapProductEvent is IAPPurchaseFailed iapPurchaseFailed)
                    {
                        Implementer.TrackDesignEvent($"IAP:PurchaseFailed:{iapPurchaseFailed.ProductId}");
                    }
                }
                else if (iapEvent is IAPSuggested iapSuggested)
                {
                    Implementer.TrackDesignEvent($"IAP:Suggested:{iapSuggested.ProductId}");
                }
                else if (iapEvent is IAPCurrentlyActive iapCurrentlyActive)
                {
                    foreach (string activeProductId in iapCurrentlyActive.ActiveProductIds)
                    {
                        Implementer.TrackDesignEvent($"IAP:ProductActive:{activeProductId}");
                    }
                }
            }
        }

        private void TranslateAdEventToLegacy(AnalyticsEvent analyticsEvent)
        {
            if (analyticsEvent is AdEvent adEvent)
            {
                if (adEvent is BannerAdEvent bannerAdEvent)
                {
                    if (bannerAdEvent is BannerAdClicked bannerAdClicked)
                    {
                        Implementer.TrackDesignEvent($"Banners:Clicked:{bannerAdClicked.AdPlacementType}");
                    }
                    else if (bannerAdEvent is BannerAdLoadFailed bannerAdLoadFailed)
                    {
                        Implementer.TrackDesignEvent($"Banners:LoadFailed:{bannerAdLoadFailed.AdPlacementType}");
                    }
                }
                else if (adEvent is InterstitialAdEvent interstitialAdEvent)
                {
                    if (interstitialAdEvent is InterstitialAdTriggered interstitialAdTriggered)
                    {
                        Implementer.TrackDesignEvent($"Interstitial:Triggered:{interstitialAdTriggered.InterstitialAdName}:{interstitialAdTriggered.LevelId}:{interstitialAdTriggered.AdPlacementType}");
                    }
                    else if (interstitialAdEvent is InterstitialAdLoadFailed interstitialAdLoadFailed)
                    {
                        Implementer.TrackDesignEvent($"Interstitials:LoadFailed:{interstitialAdLoadFailed.InterstitialAdName}:{interstitialAdLoadFailed.LevelId}:{interstitialAdLoadFailed.AdPlacementType}");
                    }
                    else if (interstitialAdEvent is InterstitialAdReady interstitialAdReady)
                    {
                        Implementer.TrackDesignEvent($"Interstitials:Ready:{interstitialAdReady.InterstitialAdName}:{interstitialAdReady.LevelId}:{interstitialAdReady.AdPlacementType}");
                    }
                    else if (interstitialAdEvent is InterstitialAdClosed interstitialAdClosed)
                    {
                        Implementer.TrackDesignEvent($"Interstitials:Closed:{interstitialAdClosed.InterstitialAdName}:{interstitialAdClosed.LevelId}:{interstitialAdClosed.AdPlacementType}");
                    }
                    else if (interstitialAdEvent is InterstitialAdClicked interstitialAdClicked)
                    {
                        Implementer.TrackDesignEvent($"Interstitials:Clicked:{interstitialAdClicked.InterstitialAdName}:{interstitialAdClicked.LevelId}:{interstitialAdClicked.AdPlacementType}");
                    }
                    else if (interstitialAdEvent is InterstitialAdShowFailed interstitialAdShowFailed)
                    {
                        Implementer.TrackDesignEvent($"Interstitials:ShowFailed:{interstitialAdShowFailed.InterstitialAdName}:{interstitialAdShowFailed.LevelId}:{interstitialAdShowFailed.AdPlacementType}");
                    }
                    else if (interstitialAdEvent is InterstitialAdShowSucceeded interstitialAdShowSucceeded)
                    {
                        Implementer.TrackDesignEvent($"Interstitials:ShowSucceeded:{interstitialAdShowSucceeded.InterstitialAdName}:{interstitialAdShowSucceeded.LevelId}:{interstitialAdShowSucceeded.AdPlacementType}");
                    }
                    else if (interstitialAdEvent is InterstitialAdFirstWatchedEver interstitialAdFirstWatchedEver)
                    {
                        Implementer.TrackDesignEvent($"Interstitials:FirstWatched:{interstitialAdFirstWatchedEver.InterstitialAdName}:{interstitialAdFirstWatchedEver.LevelId}:{interstitialAdFirstWatchedEver.AdPlacementType}", interstitialAdFirstWatchedEver.GameplaySeconds);
                    }
                }
                else if (adEvent is RewardedAdEvent rewardedAdEvent)
                {
                    if (rewardedAdEvent is RewardedVideoAdSuggested rewardedVideoAdSuggested)
                    {
                        Implementer.TrackDesignEvent($"Rewarded:Suggested:{rewardedVideoAdSuggested.RewardedAdName}:{rewardedVideoAdSuggested.LevelId}:{rewardedVideoAdSuggested.AdPlacementType}");
                    }
                    else if (rewardedAdEvent is RewardedVideoAdTriggered rewardedVideoAdTriggered)
                    {
                        Implementer.TrackDesignEvent($"Rewarded:Triggered:{rewardedVideoAdTriggered.RewardedAdName}:{rewardedVideoAdTriggered.LevelId}:{rewardedVideoAdTriggered.AdPlacementType}");
                    }
                    else if (rewardedAdEvent is RewardedAdTaken rewardedAdTaken)
                    {
                        Implementer.TrackDesignEvent($"Rewarded:Taken:{rewardedAdTaken.RewardedAdName}:{rewardedAdTaken.LevelId}:{rewardedAdTaken.AdPlacementType}");
                    }
                    else if (rewardedAdEvent is RewardedAdFirstWatchedSession rewardedAdFirstWatchedSession)
                    {
                        Implementer.TrackDesignEvent($"Rewarded:FirstWatchedSession:{rewardedAdFirstWatchedSession.RewardedAdName}:{rewardedAdFirstWatchedSession.AdPlacementType}", rewardedAdFirstWatchedSession.GameplaySeconds);
                    }
                    else if (rewardedAdEvent is RewardedAdFirstWatchedEver rewardedAdFirstWatchedEver)
                    {
                        Implementer.TrackDesignEvent($"Rewarded:FirstWatched:{rewardedAdFirstWatchedEver.RewardedAdName}:{rewardedAdFirstWatchedEver.AdPlacementType}", rewardedAdFirstWatchedEver.GameplaySeconds);
                    }
                    else if (rewardedAdEvent is RewardedAdShowFailed rewardedAdShowFailed)
                    {
                        Implementer.TrackDesignEvent($"Rewarded:Failed:{rewardedAdShowFailed.RewardedAdName}:{rewardedAdShowFailed.LevelId}:{rewardedAdShowFailed.AdPlacementType}");
                    }
                    else if (rewardedAdEvent is RewardedAdOpened rewardedAdOpened)
                    {
                        Implementer.TrackDesignEvent($"Rewarded:Opened:{rewardedAdOpened.RewardedAdName}:{rewardedAdOpened.LevelId}:{rewardedAdOpened.AdPlacementType}");
                    }
                    else if (rewardedAdEvent is RewardedAdClosed rewardedAdClosed)
                    {
                        Implementer.TrackDesignEvent($"Rewarded:Closed:{rewardedAdClosed.RewardedAdName}:{rewardedAdClosed.LevelId}:{rewardedAdClosed.AdPlacementType}");
                    }
                }
            }
        }

        private void TranslateCustomEventToLegacy(AnalyticsEvent analyticsEvent)
        {
            if (analyticsEvent is CustomEvent customEvent)
            {
                Implementer.TrackDesignEvent($"{customEvent.EventName}");
            }
        }

        private void TranslateDesignEventToLegacy(AnalyticsEvent analyticsEvent)
        {
            if (analyticsEvent is DesignEvent designEvent)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(designEvent.EventName);
                if (designEvent.Dimensions.TryGetValue("key1", out object key1))
                {
                    sb.Append($":{key1}");
                }
                if (designEvent.Dimensions.TryGetValue("key2", out object key2))
                {
                    sb.Append($":{key2}");
                }
                if (designEvent.Dimensions.TryGetValue("key3", out object key3))
                {
                    sb.Append($":{key3}");
                }
                if (designEvent.Dimensions.TryGetValue("key4", out object key4))
                {
                    sb.Append($":{key4}");
                }
                if (designEvent.Dimensions.TryGetValue("key5", out object key5))
                {
                    sb.Append($":{key5}");
                }
                
                if (designEvent.Dimensions.TryGetValue("score", out object score))
                {
                    Implementer.TrackDesignEvent(sb.ToString(), (float)score);
                }
                else
                {
                    Implementer.TrackDesignEvent(sb.ToString());
                }
            }
        }

        private void TranslateSystemEventToLegacy(AnalyticsEvent analyticsEvent)
        {
            if (analyticsEvent is HomaBellyInitialized homaBellyInitialized)
            {
                Implementer.TrackDesignEvent("HomaBelly_Initialized", Mathf.Max(0, homaBellyInitialized.TotalGameplaySeconds));
            }
            else if (analyticsEvent is AudioMuteStatus audioMuteStatus)
            {
                Implementer.TrackDesignEvent("Audio:" + (audioMuteStatus.MuteStatus == "muted" ? "Muted" : "Unmuted"));
            }
            else if (analyticsEvent is NetworkReachability networkReachability)
            {
                Implementer.TrackDesignEvent("NetworkReachability:" + (networkReachability.Reachability == "reachable" ? "Reachable" : "NotReachable"));
            }
        }

        private void TranslateSessionEventToLegacy(AnalyticsEvent analyticsEvent)
        {
            if (analyticsEvent is SessionEvent sessionEvent)
            {
                if (analyticsEvent is MenuOpened menuOpened)
                {
                    Implementer.TrackDesignEvent($"Menu:{menuOpened.MenuName}:Opened");
                }
                else if (analyticsEvent is MenuClosed menuClosed)
                {
                    Implementer.TrackDesignEvent($"Menu:{menuClosed.MenuName}:Closed");
                }
                else if (analyticsEvent is MainMenuLoaded mainMenuLoaded)
                {
                    Implementer.TrackDesignEvent("MainMenu_Loaded", mainMenuLoaded.TotalGameplaySeconds);
                }
                else if (analyticsEvent is GameplayStarted gameplayStarted)
                {
                    Implementer.TrackDesignEvent("GamePlay_Started", gameplayStarted.TotalGameplaySeconds);
                }
                else if (analyticsEvent is GameLaunched gameLaunched)
                {
                    if (! IsForwardingToHomaAnalytics)
                        Implementer.TrackDesignEvent("GameLaunched");
                }
                else if (analyticsEvent is SessionPlayed sessionPlayed)
                {
                    if (! IsForwardingToHomaAnalytics)
                        Implementer.TrackDesignEvent("Session:" + sessionPlayed.SessionNumber + ":Played", sessionPlayed.SessionLength);
                }
                else if (analyticsEvent is SessionStarted sessionStarted)
                {
                    if (! IsForwardingToHomaAnalytics)
                        Implementer.TrackDesignEvent("Session:" + sessionStarted.SessionNumber + ":Started", sessionStarted.OfflineTime);
                }
            }
        }

        private void TranslateProgressionEventToLegacy(AnalyticsEvent analyticsEvent)
        {
            if (analyticsEvent is ProgressionEvent progressionEvent)
            {
                if (progressionEvent is LevelStarted levelStarted)
                {
                    Implementer.TrackProgressionEvent(ProgressionStatus.Start, "Level_" + levelStarted.LevelId);
                }
                else if (progressionEvent is LevelCompleted levelCompleted)
                {
                    Implementer.TrackProgressionEvent(ProgressionStatus.Complete, "Level_" + levelCompleted.LevelId);
                }
                else if (progressionEvent is LevelFirstCompletion levelFirstCompletion)
                {
                    if (! IsForwardingToHomaAnalytics)
                    {
                        Implementer.TrackDesignEvent("Levels:Duration:" + levelFirstCompletion.LevelId,
                            levelFirstCompletion.LevelDuration);
                        Implementer.TrackDesignEvent("Levels:Attempts:" + levelFirstCompletion.LevelId,
                            levelFirstCompletion.Attempts);
                    }
                }
                else if (progressionEvent is LevelFailed levelFailed)
                {
                    Implementer.TrackProgressionEvent(ProgressionStatus.Fail, "Level_" + levelFailed.LevelId);
                }
                else if (progressionEvent is Checkpoint checkpoint)
                {
                    Implementer.TrackDesignEvent($"Checkpoint:{checkpoint.CheckpointId}");
                }
                else if (progressionEvent is BonusObject bonusObject)
                {
                    Implementer.TrackDesignEvent($"Bonus:{bonusObject.BonusObjectType}:{bonusObject.BonusObjectName}:Level{bonusObject.LevelId}");
                }
                else if (progressionEvent is MissionEvent missionEvent)
                {
                    if (missionEvent is MissionCompleted missionCompleted)
                    {
                        Implementer.TrackDesignEvent($"Mission:{missionCompleted.MissionId}:Completed");
                    }
                    else if (missionEvent is MissionFailed missionFailed)
                    {
                        Implementer.TrackDesignEvent($"Mission:{missionFailed.MissionId}:Failed");
                    }
                    else if (missionEvent is MissionStarted missionStarted)
                    {
                        Implementer.TrackDesignEvent($"Mission:{missionStarted.MissionId}:Started");
                    }
                }
            }
            else if (analyticsEvent is TutorialEvent tutorialEvent)
            {
                if (tutorialEvent is TutorialStepComplete tutorialStepComplete)
                {
                    Implementer.TrackDesignEvent("Tutorial:" + tutorialStepComplete.TutorialStep + ":Completed", tutorialStepComplete.StepDuration);
                }
                else if (tutorialEvent is TutorialStepFailed tutorialStepFailed)
                {
                    Implementer.TrackDesignEvent("Tutorial:" + tutorialStepFailed.TutorialStep + ":Failed");
                }
                else if (tutorialEvent is TutorialStepStarted tutorialStepStarted)
                {
                    Implementer.TrackDesignEvent("Tutorial:" + tutorialStepStarted.TutorialStep + ":Started", tutorialStepStarted.GameplayTime);
                }
            }
        }
    }
}
