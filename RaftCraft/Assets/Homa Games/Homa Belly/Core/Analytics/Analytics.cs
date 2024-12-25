using System;
using System.Collections.Generic;
using System.Diagnostics;
using HomaGames.HomaBelly.Internal.Analytics;
using JetBrains.Annotations;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// Class acting as the main entry point for the Analytics public API in Homa Belly.
    /// </summary>
    public static class Analytics
    {
        
        private const string LEVEL_ATTEMPT_KEY = "com.homagames.homabelly.level_{0}_attempt";
        private const string LEVEL_STARTED_AT_GAMEPLAY_SECONDS_KEY = "com.homagames.homabelly.level_started_{0}_at_gameplay_seconds";
        private const string LEVEL_COMPLETED_KEY = "com.homagames.homabelly.level_completed_{0}_at_gameplay_seconds";
        private const string CURRENT_LEVEL_KEY = "com.homagames.homabelly.current_level";

        private const string TUTORIAL_STEP_STARTED_KEY = "com.homagames.homabelly.tutorial_step_started_{0}_at_gameplay_seconds";
        private const string TUTORIAL_STEP_COMPLETED_KEY = "com.homagames.homabelly.tutorial_step_completed_{0}_at_gameplay_seconds";
        private const string CURRENT_TUTORIAL_STEP_KEY = "com.homagames.homabelly.current_tutorial_step";

        private const string REWARDED_AD_FIRST_TIME_TAKEN_EVER_KEY = "com.homagames.homabelly.rewarded_ad_first_time_taken_ever_{0}";
        private const string INTERSTITIAL_AD_FIRST_TIME_KEY = "com.homagames.homabelly.interstitial_ad_first_time_{0}";

        
        private const string CURRENT_GAMEPLAY_TIME_KEY = "com.homagames.homabelly.current_gameplay_seconds";
        private const string MAIN_MENU_LOADED_KEY = "com.homagames.homabelly.main_menu_loaded";
        private const string GAMEPLAY_STARTED_KEY = "com.homagames.homabelly.gameplay_started";

        private static int _currentLevelId = 1;
        private static int _currentTutorialStep = 1;
        private static string currentGameplayTime;
        private static long gameplayResumedTimestamp;
        private static string _sanitizedCurrentRewardedAdName = "Default";
        private static string _sanitizedCurrentInterstitialAdName = "Default";
        private static string _latestRewardedAdTriggeredImpressionId;
        private static string _latestInterstitialAdImpressionId;
        private static string _latestBannerAdImpressionId;
        private static AdInfo _currentBannerAdInfo;
        private static Dictionary<string, bool> rewardedAdsTakenThisSession = new Dictionary<string, bool>();
        private static Dictionary<string, string> _suggestedRewardedImpressionsDictionary = new Dictionary<string, string>();

        static Analytics()
        {
            Start();
        }

        public static void Start()
        {
            // Recover any previous level or tutorial step stored
            _currentLevelId = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);
            _currentTutorialStep = PlayerPrefs.GetInt(CURRENT_TUTORIAL_STEP_KEY, 1);
            
            currentGameplayTime = PlayerPrefs.GetString(CURRENT_GAMEPLAY_TIME_KEY, "");
            gameplayResumedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            if (HomaBelly.Instance.IsInitialized)
            {
                OnHomaBellyInitialized();
            }
            else
            {
                Events.onInitialized += OnHomaBellyInitialized;    
            }
        }
       
        [NotNull, ItemNotNull, Pure]
        private static List<AnalyticsBase> GetAllAnalyticsDependencies()
        {
#if UNITY_EDITOR
            return new List<AnalyticsBase>();
#else
            return HomaBridgeDependencies.GetAnalytics(out var analytics) ? analytics : new List<AnalyticsBase>();
#endif
        }

        private static void OnHomaBellyInitialized()
        {
            RegisterAdEvents();
        }
        
        public static void OnApplicationPause(bool pause)
        {
#if !HOMA_DISABLE_AUTO_ANALYTICS
            // Game is paused
            if (pause)
            {
                // Save new current gameplay time
                PlayerPrefs.SetString(CURRENT_GAMEPLAY_TIME_KEY, GetTotalGameplaySecondsAtThisExactMoment().ToString());
                PlayerPrefs.Save();
            }
            else
            {
                // Game is resumed
                currentGameplayTime = PlayerPrefs.GetString(CURRENT_GAMEPLAY_TIME_KEY, "");
                gameplayResumedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
#endif
        }
        
        /// <summary>
        /// Tracks a fully customized event. Name and parameters can be any desired information.
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="parameters">Any desired custom information as a Dictionary</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId CustomEvent(string eventName, Dictionary<string, object> parameters)
        {
            return new CustomEvent(eventName, parameters).TrackEvent();
        }
        
        /// <summary>
        /// Tracks a design event. Name can be any desired one, and designDimensions is an optional object with up to 5 strings and a float. 
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="designDimensions">Any desired custom information as a Dictionary</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId DesignEvent(string eventName, DesignDimensions designDimensions = null)
        {
            return new DesignEvent(eventName, designDimensions).TrackEvent();
        }
        
        /// <summary>
        /// Invoke this whenever there's a resource flow (source/sink) event.
        /// </summary>
        /// <param name="flowType">`Source` when user obtains some resource, or `Sink` when user spends some resource</param>
        /// <param name="currency">Resource name</param>
        /// <param name="flowAmount">Quantity affected by the flow</param>
        /// <param name="finalAmount">Final amount after the flow transaction</param>
        /// <param name="flowReason">Reason triggering the flow transaction</param>
        /// <param name="triggeredByEventId">Any previous EventId triggering this ResourceFlowEvent. ie: LevelCompleted event triggers ResourceFlowEvent</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId ResourceFlowEvent(ResourceFlowType flowType, string currency, float flowAmount, float finalAmount, ResourceFlowReason flowReason, EventId triggeredByEventId = null)
        {
            if (flowAmount < 0)
            {
                HomaGamesLog.Error("[Analytics] flowAmount cannot be negative. If you want to track subtraction transaction, please use ResourceFlowType.Sink with absolute flowAmount.");
                return default!;
            }
            
            string reference = "";
            switch (flowReason)
            {
                case ResourceFlowReason.RewardedVideoAd:
                    reference = _latestRewardedAdTriggeredImpressionId;
                    break;
                case ResourceFlowReason.InterstitialAd:
                    reference = _latestInterstitialAdImpressionId;
                    break;
                case ResourceFlowReason.InAppPurchase:
                    // TODO: reference transactionID
                    break;
                case ResourceFlowReason.Progression:
                    reference = triggeredByEventId?.ToString();
                    break;
                case ResourceFlowReason.Other:
                    reference = triggeredByEventId != null ? triggeredByEventId.ToString() : "Other";
                    break;
                default:
                    // NO-OP
                    break;
            }
            
            return new ResourceFlow(flowType, currency, flowAmount, finalAmount, flowReason, reference).TrackEvent();
        }
        
        /// <summary>
        /// Invoke this whenever the user gets an item
        /// </summary>
        /// <param name="itemId">An identifier for the obtained item</param>
        /// <param name="itemLevel">The level of the obtained item</param>
        /// <param name="reason">Reason triggering the item transaction</param>
        /// <param name="triggeredByEventId">Any previous EventId triggering this ItemObtained. ie: LevelCompleted event triggers ItemObtained</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId ItemObtained(string itemId, int itemLevel, ItemFlowReason reason, EventId triggeredByEventId = null)
        {
            string reference = "";
            switch (reason)
            {
                case ItemFlowReason.RewardedVideoAd:
                    reference = _latestRewardedAdTriggeredImpressionId;
                    break;
                case ItemFlowReason.InterstitialAd:
                    reference = _latestInterstitialAdImpressionId;
                    break;
                case ItemFlowReason.InAppPurchase:
                    // TODO: reference transactionID
                    break;
                case ItemFlowReason.Progression:
                    reference = triggeredByEventId?.ToString();
                    break;
                case ItemFlowReason.Other:
                    reference = triggeredByEventId != null ? triggeredByEventId.ToString() : "Other";
                    break;
                default:
                    // NO-OP
                    break;
            }
            
            return new ItemObtained(itemId, itemLevel, reason, reference).TrackEvent();
        }
        
        /// <summary>
        /// Invoke this whenever the user upgrades an item or gets a general (non-item) upgrade
        /// </summary>
        /// <param name="upgradeType">'Item' if it's an item upgrade, 'Upgrade' for general non-item upgrades</param>
        /// <param name="itemId">The identifier for the upgraded item or general upgrade</param>
        /// <param name="itemLevel">The level of the upgraded item</param>
        /// <param name="reason">Reason triggering the upgrade</param>
        /// <param name="triggeredByEventId">Any previous EventId triggering this ItemUpgraded. ie: ResourceFlowEvent (sink) triggers ItemUpgraded</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId ItemUpgraded(ItemUpgradeType upgradeType, string itemId, int itemLevel, ItemFlowReason reason, EventId triggeredByEventId = null)
        {
            string reference = "";
            switch (reason)
            {
                case ItemFlowReason.RewardedVideoAd:
                    reference = _latestRewardedAdTriggeredImpressionId;
                    break;
                case ItemFlowReason.InterstitialAd:
                    reference = _latestInterstitialAdImpressionId;
                    break;
                case ItemFlowReason.InAppPurchase:
                    // TODO: reference transactionID
                    break;
                case ItemFlowReason.Progression:
                    reference = triggeredByEventId?.ToString();
                    break;
                case ItemFlowReason.Other:
                    reference = triggeredByEventId != null ? triggeredByEventId.ToString() : "Other";
                    break;
                default:
                    // NO-OP
                    break;
            }
            
            return new ItemUpgraded(itemId, itemLevel, upgradeType, reason, reference).TrackEvent();    
        }
        
        /// <summary>
        /// Invoke this whenever the user consumes an item
        /// </summary>
        /// <param name="itemId">An identifier for the consumed item</param>
        /// <param name="itemLevel">The level of the consumed item</param>
        /// <param name="reason">Reason triggering the consumption of the item</param>
        /// <param name="triggeredByEventId">Any previous EventId triggering this ItemConsumed. ie: ItemObtained (skin) event triggers ItemConsumed (coins)</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId ItemConsumed(string itemId, int itemLevel, ItemFlowReason reason, EventId triggeredByEventId = null)
        {
            string reference = "";
            switch (reason)
            {
                case ItemFlowReason.RewardedVideoAd:
                    reference = _latestRewardedAdTriggeredImpressionId;
                    break;
                case ItemFlowReason.InterstitialAd:
                    reference = _latestInterstitialAdImpressionId;
                    break;
                case ItemFlowReason.InAppPurchase:
                    // TODO: reference transactionID
                    break;
                case ItemFlowReason.Progression:
                    reference = triggeredByEventId?.ToString();
                    break;
                case ItemFlowReason.Other:
                    reference = triggeredByEventId != null ? triggeredByEventId.ToString() : "Other";
                    break;
                default:
                    // NO-OP
                    break;
            }
            
            return new ItemConsumed(itemId, itemLevel, reason, reference).TrackEvent();
        }

        /// <summary>
        /// Invoke this every time the player starts a level
        /// </summary>
        /// <param name="levelId">A sequential number for the level. Please start at 1.</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId LevelStarted(int levelId)
        {
            // Set current level
            _currentLevelId = levelId;
            PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, _currentLevelId);
            
            // Set level attempt
            int levelAttempt = PlayerPrefs.GetInt(string.Format(LEVEL_ATTEMPT_KEY, _currentLevelId), 0);
            levelAttempt++;
            PlayerPrefs.SetInt(string.Format(LEVEL_ATTEMPT_KEY,_currentLevelId), levelAttempt);

            // GameplayTime is the time spent in the game since the first launch, in seconds.
            long totalGameplaySecondsAtThisMoment = GetTotalGameplaySecondsAtThisExactMoment();

            // Save the GameplayTime of the level started attempt
            PlayerPrefs.SetString(string.Format(LEVEL_STARTED_AT_GAMEPLAY_SECONDS_KEY, _currentLevelId), totalGameplaySecondsAtThisMoment.ToString());
            
            // Save Player Prefs
            PlayerPrefs.Save();
            
            return new LevelStarted(_currentLevelId).TrackEvent();
        }
        
        /// <summary>
        /// Invoke this every time the player fails the level
        /// </summary>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId LevelFailed()
        {
            return new LevelFailed(_currentLevelId).TrackEvent();
        }
        
        /// <summary>
        /// Invoke this every time the player completes the level
        /// </summary>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId LevelCompleted()
        {
            EventId eventId = new LevelCompleted(_currentLevelId).TrackEvent();
            AttributionEventCounter.SetValue(AttributionEventCounter.EventType.LevelAchieved, _currentLevelId);
            
            // If is the very first time this level is completed, track it
            if (!PlayerPrefs.HasKey(string.Format(LEVEL_COMPLETED_KEY, _currentLevelId)))
            {
                // LevelDuration is the time spent in the level from the start until the completion, in seconds.
                long levelStartAtGameplaySeconds = long.Parse(PlayerPrefs.GetString(string.Format(LEVEL_STARTED_AT_GAMEPLAY_SECONDS_KEY, _currentLevelId), "0"));
                long levelDuration = Math.Max(0, GetTotalGameplaySecondsAtThisExactMoment() - levelStartAtGameplaySeconds);
                PlayerPrefs.SetString(string.Format(LEVEL_COMPLETED_KEY, _currentLevelId), GetTotalGameplaySecondsAtThisExactMoment().ToString());
                PlayerPrefs.Save();
                
                // Track number of attempts before completing the level
                new LevelFirstCompletion(_currentLevelId, levelDuration, PlayerPrefs.GetInt(string.Format(LEVEL_ATTEMPT_KEY, _currentLevelId), 1)).TrackEvent();
            }

            return eventId;
        }

        /// <summary>
        /// Invoke this method every time a tutorial step is started. Invoking
        /// it twice for the same step is harmless, as only the very first
        /// one will be taken into account.
        /// </summary>
        /// <param name="step">The tutorial step</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId TutorialStepStarted(int step)
        {
            // Set current level
            _currentTutorialStep = step;
            PlayerPrefs.SetInt(CURRENT_TUTORIAL_STEP_KEY, _currentTutorialStep);
            PlayerPrefs.Save();

            // If is the very first time this tutorial step is started, track it
            if (!PlayerPrefs.HasKey(string.Format(TUTORIAL_STEP_STARTED_KEY, _currentTutorialStep)))
            {
                // GameplayTime is the time spent in the game since the first launch, in seconds.
                long totalGameplaySecondsAtThisMoment = GetTotalGameplaySecondsAtThisExactMoment();
                PlayerPrefs.SetString(string.Format(TUTORIAL_STEP_STARTED_KEY, _currentTutorialStep), totalGameplaySecondsAtThisMoment.ToString());
                PlayerPrefs.Save();
                
                return new TutorialStepStarted(_currentTutorialStep, totalGameplaySecondsAtThisMoment).TrackEvent();
            }

            return default!;
        }
        
        /// <summary>
        /// When the player does not execute the asked behavior in the current tutorial step
        /// </summary>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId TutorialStepFailed()
        {
            return new TutorialStepFailed(_currentTutorialStep).TrackEvent();
        }
        
        /// <summary>
        /// Invoke this method every time a tutorial step is completed. Invoking
        /// it twice for the same step is harmless, as only the very first
        /// one will be taken into account.
        /// </summary>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId TutorialStepCompleted()
        {
            // If is the very first time this tutorial step is completed, track it
            if (!PlayerPrefs.HasKey(string.Format(TUTORIAL_STEP_COMPLETED_KEY, _currentTutorialStep)))
            {
                // StepDuration is the time spent to complete the step, in seconds.
                long tutorialStepStartAtGameplaySeconds = long.Parse(PlayerPrefs.GetString(string.Format(TUTORIAL_STEP_STARTED_KEY, _currentTutorialStep), "0"));
                long tutorialStepDuration = Math.Max(0, GetTotalGameplaySecondsAtThisExactMoment() - tutorialStepStartAtGameplaySeconds);
                
                PlayerPrefs.SetString(string.Format(TUTORIAL_STEP_COMPLETED_KEY, _currentTutorialStep), GetTotalGameplaySecondsAtThisExactMoment().ToString());
                PlayerPrefs.Save();
                
                return new TutorialStepComplete(_currentTutorialStep, tutorialStepDuration).TrackEvent();
            }
            
            return default!;
        }

        /// <summary>
        /// Invoke this method when the player reaches a certain point of interest in gameplay
        /// </summary>
        /// <param name="checkpointId">A string identifying the point reached</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId Checkpoint(string checkpointId)
        {
            return new Checkpoint(checkpointId, _currentLevelId).TrackEvent();
        }

        /// <summary>
        /// Invoke this method when the player finishes the endgame
        /// </summary>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId GameEnded()
        {
            return new Checkpoint("game_ended", _currentLevelId).TrackEvent();
        }

        /// <summary>
        /// Invoke this method when a mission is open for a user
        /// </summary>
        /// <param name="missionId">The ID for the mission</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId MissionStarted(string missionId)
        {
            return new MissionStarted(missionId, _currentLevelId).TrackEvent();
        }

        /// <summary>
        /// Invoke this method when a mission is completed
        /// </summary>
        /// <param name="missionId">The ID for the mission</param>
        /// <param name="reward">The reward given for completing the mission</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId MissionCompleted(string missionId, string reward)
        {
            return new MissionCompleted(missionId, reward, _currentLevelId).TrackEvent();
        }

        /// <summary>
        /// Invoke this method when a mission is skipped or failed
        /// </summary>
        /// <param name="missionId">The ID for the mission</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId MissionFailed(string missionId)
        {
            return new MissionFailed(missionId, _currentLevelId).TrackEvent();
        }
        
        /// <summary>
        /// Invoke this method when a bonus object is opened
        /// </summary>
        /// <param name="bonusObjectType">The type of bonus (chestroom, randombonus, bonuslevel...)</param>
        /// <param name="bonusObjectName">An identifier for the bonus object (chestroom position id, name of the bonus object...)</param>
        /// <param name="reward">The reward given for the bonus object</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId BonusObjectOpened(BonusObjectType bonusObjectType, string bonusObjectName, string reward)
        {
            return new BonusObject(bonusObjectType, bonusObjectName, reward, _currentLevelId).TrackEvent();
        }
        
        /// <summary>
        /// Invoke this method whenever a rewarded offer is suggested to the player.
        /// </summary>
        /// <param name="placementName">Please follow the nomenclature in the relevant document</param>
        /// <param name="adPlacementType">The type of placement</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId SuggestedRewardedAd(string placementName,AdPlacementType adPlacementType = AdPlacementType.Default)
        {
            _sanitizedCurrentRewardedAdName = Sanitize(placementName);
            string currentRewardedAdImpressionId = GenerateNewImpressionId();
            _suggestedRewardedImpressionsDictionary[_sanitizedCurrentRewardedAdName] = currentRewardedAdImpressionId;
            return new RewardedVideoAdSuggested(_sanitizedCurrentRewardedAdName, currentRewardedAdImpressionId, _currentLevelId, adPlacementType).TrackEvent();
        }
        
        /// <summary>
        /// Internally invoked by Homa Belly upon showing a Rewarded Video
        /// </summary>
        /// <param name="placementName"></param>
        /// <param name="adPlacementType"></param>
        public static void RewardedAdTriggered(string placementName,AdPlacementType adPlacementType)
        {
#if !HOMA_DISABLE_AUTO_ANALYTICS
            _sanitizedCurrentRewardedAdName = Sanitize(placementName);
            string impressionId = GetImpressionId(_sanitizedCurrentRewardedAdName);
            if (_suggestedRewardedImpressionsDictionary.TryGetValue(placementName + "_triggered", out string lastGeneratedId) && lastGeneratedId == impressionId)
            {
                //RewardedAdTriggered was called twice without a SuggestedRewardedAd invocation. We need to generate a new impressionId.
                impressionId = GenerateNewImpressionId();
                _suggestedRewardedImpressionsDictionary[placementName] = impressionId;
            }

            _latestRewardedAdTriggeredImpressionId = impressionId;
            _suggestedRewardedImpressionsDictionary[placementName + "_triggered"] = impressionId;
            new RewardedVideoAdTriggered(_sanitizedCurrentRewardedAdName, impressionId, _currentLevelId, adPlacementType).TrackEvent();
#endif
        }
        
        /// <summary>
        /// Internally invoked by Homa Belly upon showing an Interstitial Video
        /// </summary>
        /// <param name="placementName"></param>
        /// <param name="adPlacementType"></param>
        public static void InterstitialAdTriggered(string placementName,AdPlacementType adPlacementType)
        {
#if !HOMA_DISABLE_AUTO_ANALYTICS
            _sanitizedCurrentInterstitialAdName = Sanitize(placementName);
            _latestInterstitialAdImpressionId = GenerateNewImpressionId();
            new InterstitialAdTriggered(_sanitizedCurrentInterstitialAdName, _latestInterstitialAdImpressionId, _currentLevelId, adPlacementType).TrackEvent();
#endif
        }
        
        /// <summary>
        /// Invoke this method every time your Main Menu screen is loaded
        /// </summary>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId MainMenuLoaded()
        {
            return new MainMenuLoaded(GetTotalGameplaySecondsAtThisExactMoment()).TrackEvent();
        }
        
        /// <summary>
        /// Invoke this method every time the user starts the gameplay
        /// </summary>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId GameplayStarted()
        {
            return new GameplayStarted(GetTotalGameplaySecondsAtThisExactMoment()).TrackEvent();
        }

        /// <summary>
        /// Invoke this method when the user enters a menu.
        /// </summary>
        /// <param name="menuName">The name of the menu</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId MenuOpened(string menuName)
        {
            return new MenuOpened(menuName).TrackEvent();
        }

        /// <summary>
        /// Invoke this method when the user leaves a menu.
        /// </summary>
        /// <param name="menuName">The name of the menu</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId MenuClosed(string menuName)
        {
            return new MenuClosed(menuName).TrackEvent();
        }
        
        /// <summary>
        /// Invoke this method when appropriate to track internal packages events as instructed by PMs.
        /// </summary>
        /// <param name="name">The name of the package being tracked</param>
        /// <param name="version">The version number of the package</param>
        /// <param name="status">"Installed" when a package is installed in a game,
        /// "Enabled" when a package is installed in a game and enabled,
        /// "Suggested" when the package is shown to the player,
        /// "Triggered" when the player interacts right after sending suggested event
        /// "Interacted" when the player interacts with the package (click any button)</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId InternalPackageEvent(string name, string version, InternalPackageStatus status)
        {
            return new InternalPackage(name, version, status).TrackEvent();
        }

        /// <summary>
        /// Invoke this method when a popup suggesting an IAP is shown to the user
        /// </summary>
        /// <param name="popupName">The name identifying the popup that was shown</param>
        /// <param name="productId">The productId of the IAP suggested</param>
        /// <returns>EventId to be referenced, if necessary, in later events as triggers</returns>
        [PublicAPI]
        [NotNull]
        public static EventId IAPSuggested(string popupName, string productId = null)
        {
            return new IAPSuggested(popupName, productId).TrackEvent();
        }

        /// <summary>
        /// Avoids sending an empty parameter to analytics. Also replaces spaces with underscores.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static string Sanitize(string parameter)
        {
            return string.IsNullOrWhiteSpace(parameter) ? "_" : parameter.Replace(" ", "_");
        }
        
        /// <summary>
        /// Obtains the current gameplay elapsed time in seconds at the
        /// moment of invoking this method.
        ///
        /// This takes into account only the time spent playing, not the time
        /// when the application is paused (the user minimized/exitted the game)
        /// </summary>
        /// <returns></returns>
        private static long GetTotalGameplaySecondsAtThisExactMoment()
        {
            // Calculate time since last resume
            long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long gameplayTimeSinceApplicationResumed = currentTimestamp - gameplayResumedTimestamp;

            // Parse and add calculated time to current gameplay time
            long currentGameplayTimeLong = string.IsNullOrEmpty(currentGameplayTime) ? 0 : long.Parse(currentGameplayTime);
            currentGameplayTimeLong += gameplayTimeSinceApplicationResumed;

            return currentGameplayTimeLong;
        }
        
        /// <summary>
        /// Registers Ad Events to be tracked
        /// </summary>
        private static void RegisterAdEvents()
        {
#if !HOMA_DISABLE_AUTO_ANALYTICS
            // Banner
            Events.onBannerAdLoadedEvent += adInfo =>
            {
                _currentBannerAdInfo = adInfo;
                _latestBannerAdImpressionId = GenerateNewImpressionId();
            };
                
            Events.onBannerAdClickedEvent += (adInfo) =>
            {
                new BannerAdClicked(_latestBannerAdImpressionId, adInfo.AdPlacementType).TrackEvent();
            };

            Events.onBannerAdLoadFailedEvent += (adInfo) =>
            {
                new BannerAdLoadFailed(_latestBannerAdImpressionId, adInfo.AdPlacementType).TrackEvent();
            };

            Events.onBannerAdScreenPresentedEvent += (_) =>
            {
                new BannerAdExpanded(_latestBannerAdImpressionId, _currentBannerAdInfo.AdPlacementType).TrackEvent();
            };

            Events.onBannerAdScreenDismissedEvent += () =>
            {
                new BannerAdCollapsed(_latestBannerAdImpressionId, _currentBannerAdInfo.AdPlacementType).TrackEvent();
            };

            // Interstitial
            Events.onInterstitialAdReadyEvent += (adInfo) =>
            {
                new InterstitialAdReady(_sanitizedCurrentInterstitialAdName, _latestInterstitialAdImpressionId, _currentLevelId, adInfo.AdPlacementType).TrackEvent();
            };

            Events.onInterstitialAdLoadFailedEvent += (adInfo) =>
            {
                new InterstitialAdLoadFailed(_sanitizedCurrentInterstitialAdName, _latestInterstitialAdImpressionId, _currentLevelId, adInfo.AdPlacementType).TrackEvent();
            };

            Events.onInterstitialAdClosedEvent += (adInfo) =>
            {
                new InterstitialAdClosed(_sanitizedCurrentInterstitialAdName, _latestInterstitialAdImpressionId, _currentLevelId, adInfo.AdPlacementType).TrackEvent();
            };

            Events.onInterstitialAdClickedEvent += (adInfo) =>
            {
                new InterstitialAdClicked(_sanitizedCurrentInterstitialAdName, _latestInterstitialAdImpressionId, _currentLevelId, adInfo.AdPlacementType).TrackEvent();
            };

            Events.onInterstitialAdShowFailedEvent += (adInfo) =>
            {
                new InterstitialAdShowFailed(_sanitizedCurrentInterstitialAdName, _latestInterstitialAdImpressionId, _currentLevelId, adInfo.AdPlacementType).TrackEvent();
            };

            Events.onInterstitialAdShowSucceededEvent += (adInfo) =>
            {
                AttributionEventCounter.IncrementValue(AttributionEventCounter.EventType.IsWatched);
                new InterstitialAdShowSucceeded(_sanitizedCurrentInterstitialAdName, _latestInterstitialAdImpressionId, _currentLevelId, adInfo.AdPlacementType).TrackEvent();
                
                if (!string.IsNullOrEmpty(_sanitizedCurrentInterstitialAdName))
                {
                    if (PlayerPrefs.GetInt(string.Format(INTERSTITIAL_AD_FIRST_TIME_KEY, _sanitizedCurrentInterstitialAdName), 0) == 0)
                    {
                        new InterstitialAdFirstWatchedEver(_sanitizedCurrentInterstitialAdName, GetImpressionId(_sanitizedCurrentInterstitialAdName), _currentLevelId, adInfo.AdPlacementType, GetTotalGameplaySecondsAtThisExactMoment()).TrackEvent();
                        PlayerPrefs.SetInt(string.Format(INTERSTITIAL_AD_FIRST_TIME_KEY, _sanitizedCurrentInterstitialAdName), 1);
                        PlayerPrefs.Save();
                    }
                }
            };

            // Rewarded Video
            Events.onRewardedVideoAdRewardedEvent += (reward,adInfo) =>
            {
                AttributionEventCounter.IncrementValue(AttributionEventCounter.EventType.RvWatched);
                
                if (!string.IsNullOrEmpty(_sanitizedCurrentRewardedAdName))
                {
                    // Current rewarded ad has not already been taken this session
                    if (!rewardedAdsTakenThisSession.ContainsKey(_sanitizedCurrentRewardedAdName))
                    {
                        rewardedAdsTakenThisSession.Add(_sanitizedCurrentRewardedAdName, true);
                        new RewardedAdFirstWatchedSession(_sanitizedCurrentRewardedAdName, GetImpressionId(_sanitizedCurrentRewardedAdName), _currentLevelId, adInfo.AdPlacementType, GetTotalGameplaySecondsAtThisExactMoment()).TrackEvent();
                    }

                    // Current rewarded ad has not been taken ever
                    if (PlayerPrefs.GetInt(string.Format(REWARDED_AD_FIRST_TIME_TAKEN_EVER_KEY, _sanitizedCurrentRewardedAdName), 0) == 0)
                    {
                        new RewardedAdFirstWatchedEver(_sanitizedCurrentRewardedAdName, GetImpressionId(_sanitizedCurrentRewardedAdName), _currentLevelId, adInfo.AdPlacementType, GetTotalGameplaySecondsAtThisExactMoment()).TrackEvent();
                        PlayerPrefs.SetInt(string.Format(REWARDED_AD_FIRST_TIME_TAKEN_EVER_KEY, _sanitizedCurrentRewardedAdName), 1);
                        PlayerPrefs.Save();
                    }
                }

                new RewardedAdTaken(_sanitizedCurrentRewardedAdName, GetImpressionId(_sanitizedCurrentRewardedAdName), _currentLevelId, adInfo.AdPlacementType).TrackEvent();
            };

            Events.onRewardedVideoAdShowFailedEvent += (adInfo) =>
            {
                new RewardedAdShowFailed(_sanitizedCurrentRewardedAdName, GetImpressionId(_sanitizedCurrentRewardedAdName), _currentLevelId, adInfo.AdPlacementType).TrackEvent();
            };

            Events.onRewardedVideoAdStartedEvent += (adInfo) =>
            {
                new RewardedAdOpened(_sanitizedCurrentRewardedAdName, GetImpressionId(_sanitizedCurrentRewardedAdName), _currentLevelId, adInfo.AdPlacementType).TrackEvent();
            };

            Events.onRewardedVideoAdClosedEvent += (adInfo) =>
            {
                new RewardedAdClosed(_sanitizedCurrentRewardedAdName, GetImpressionId(_sanitizedCurrentRewardedAdName), _currentLevelId, adInfo.AdPlacementType).TrackEvent();
            };
#endif
        }
        
        private static string GetImpressionId(string rewardedAdName)
        {
            if (!_suggestedRewardedImpressionsDictionary.TryGetValue(rewardedAdName,
                    out string currentRewardedAdImpressionId))
            {
                currentRewardedAdImpressionId = GenerateNewImpressionId();
                _suggestedRewardedImpressionsDictionary[rewardedAdName] = currentRewardedAdImpressionId;
            }

            return currentRewardedAdImpressionId;
        }

        private static string GenerateNewImpressionId()
        {
            return Guid.NewGuid().ToString("N");
        }
        
        [Conditional("UNITY_EDITOR")]
        private static void Log(object[] parameters)
        {
            var str = "[Homa Belly] Tracking Event : ";

            for (var i = 0; i < parameters.Length; i += 2)
            {
                string paramName = parameters[i].ToString();

                if (i + 1 < parameters.Length)
                {
                    string paramValue = parameters[i + 1]?.ToString() ?? "null";

                    str += $"{paramName}={paramValue} ";
                }
            }

            HomaGamesLog.Debug(str.Trim());
        }
    }
}