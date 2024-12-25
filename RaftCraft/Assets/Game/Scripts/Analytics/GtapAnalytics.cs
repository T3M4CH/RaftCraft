using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Scripts.Core.Debug;
using Game.Scripts.Player.HeroPumping.Enums;
using HomaGames.HomaBelly;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using HomaAnalytics = HomaGames.HomaBelly.Analytics;

namespace GTap.Analytics
{
    public class GtapAnalytics : MonoBehaviour
    {
        public enum EventType
        {
            Undefined = 0,
            Start = 1,
            Complete = 2,
            Fail = 3,
            Restart = 4,
            Leave = 5,
        }

        private static List<LevelAnalytics> _analyticsLevels = new();

        public static LevelAnalytics GetLevel(string levelName)
        {
            if (_analyticsLevels.Any(l => l.LevelNumber == levelName))
            {
                return _analyticsLevels.FirstOrDefault(l => l.LevelNumber == levelName);
            }

            var lvl = new LevelAnalytics(levelName);
            _analyticsLevels.Add(lvl);
            return lvl;
        }

        public static void GameplayStarted()
        {
            try
            {
                HomaAnalytics.GameplayStarted();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void LevelStart(int level, int? lvlCount = null, int? attemptNum = null, int? levelLoop = null,
            string levelCollection1 = null, string levelCollection2 = null,
            string missionType = null, string missionName = null)
        {
            var levelAnalytics = GetLevel(level.ToString());
            levelAnalytics.ProgressionStart();

            UniTask.Void(async () =>
            {
                try
                {
                    var ev = new GtapGameEvent
                    {
                        GtapAnalyticsEventType = GtapAnalyticsEventType.Level,
                        EventType = EventType.Start,
                        // Required params
                        EventParams = new Dictionary<string, object>
                        {
                            { EventParam.MissionID, level.ToString() },
                            //{ EventParam.MissionAttempt, attemptNum },
                            //{ EventParam.LevelCount, lvlCount }
                        }
                    };

                    if (levelLoop.HasValue)
                    {
                        ev.EventParams.Add(EventParam.LevelLoop, levelLoop);
                    }

                    if (!string.IsNullOrEmpty(levelCollection1))
                    {
                        ev.EventParams.Add(EventParam.LevelCollection1, levelCollection1);
                    }

                    if (!string.IsNullOrEmpty(levelCollection2))
                    {
                        ev.EventParams.Add(EventParam.LevelCollection2, levelCollection2);
                    }

                    if (!string.IsNullOrEmpty(missionType))
                    {
                        ev.EventParams.Add(EventParam.MissionType, missionType);
                    }

                    if (!string.IsNullOrEmpty(missionName))
                    {
                        ev.EventParams.Add(EventParam.MissionName, missionName);
                    }

                    LogEvent(ev);
                    HomaAnalytics.LevelStarted(level);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });
        }

        private static void PostLevelTime(LevelAnalytics levelAnalytics)
        {
            UniTask.Void(async () =>
            {
                var ev = new GtapGameEvent
                {
                    GtapAnalyticsEventType = GtapAnalyticsEventType.Custom,
                    EventType = EventType.Complete,
                    // Required params
                    EventParams = new Dictionary<string, object>
                    {
                        { EventParam.MissionID, levelAnalytics.FormattedLevelName },
                        { EventParam.Message, $"{levelAnalytics.FormattedLevelName}:Time:{levelAnalytics.FormattedLevelName}" },
                        { EventParam.MessageValue, levelAnalytics.GetLevelTime() },
                    }
                };

                LogEvent(ev);
                await UniTask.Yield();
            });
        }

        public static void LevelComplete(int level, int? lvlCount = null, int? attemptNum = null, int? levelLoop = null, string levelCollection1 = null, string levelCollection2 = null,
            string missionType = null, string missionName = null, int? missionProgress = null, int? timePerLevel = null)
        {
            var levelAnalytics = GetLevel(level.ToString());

            UniTask.Void(async () =>
            {
                try
                {
                    var ev = new GtapGameEvent
                    {
                        GtapAnalyticsEventType = GtapAnalyticsEventType.Level,
                        EventType = EventType.Complete,
                        // Required params
                        EventParams = new Dictionary<string, object>
                        {
                            { EventParam.MissionID, level.ToString() },
                            //{ EventParam.MissionAttempt, attemptNum },
                            //{ EventParam.LevelCount, lvlCount }
                        }
                    };

                    if (levelLoop.HasValue)
                    {
                        ev.EventParams.Add(EventParam.LevelLoop, levelLoop);
                    }

                    if (timePerLevel.HasValue)
                    {
                        ev.EventParams.Add(EventParam.TimeValue, timePerLevel);
                    }

                    if (missionProgress.HasValue)
                    {
                        ev.EventParams.Add(EventParam.MissionProgress, missionProgress);
                    }

                    if (!string.IsNullOrEmpty(levelCollection1))
                    {
                        ev.EventParams.Add(EventParam.LevelCollection1, levelCollection1);
                    }

                    if (!string.IsNullOrEmpty(levelCollection2))
                    {
                        ev.EventParams.Add(EventParam.LevelCollection2, levelCollection2);
                    }

                    if (!string.IsNullOrEmpty(missionType))
                    {
                        ev.EventParams.Add(EventParam.MissionType, missionType);
                    }

                    if (!string.IsNullOrEmpty(missionName))
                    {
                        ev.EventParams.Add(EventParam.MissionName, missionName);
                    }

                    LogEvent(ev);
                    HomaAnalytics.LevelCompleted();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });

            //PostLevelTime(levelAnalytics);
        }

        public static void LevelFail(int level, int? lvlCount = null, int? attemptNum = null,
            int? levelLoop = null, string levelCollection1 = null, string levelCollection2 = null,
            string missionType = null, string missionName = null, int? missionProgress = null, int? timePerLevel = null)
        {
            var levelAnalytics = GetLevel(level.ToString());
            UniTask.Void(async () =>
            {
                var ev = new GtapGameEvent
                {
                    GtapAnalyticsEventType = GtapAnalyticsEventType.Level,
                    // Required params
                    EventParams = new Dictionary<string, object>
                    {
                        { EventParam.MissionID, level.ToString() },
                        { EventParam.MissionAttempt, attemptNum },
                        { EventParam.LevelCount, lvlCount }
                    }
                };

                if (levelLoop.HasValue)
                {
                    ev.EventParams.Add(EventParam.LevelLoop, levelLoop);
                }

                if (timePerLevel.HasValue)
                {
                    ev.EventParams.Add(EventParam.TimeValue, timePerLevel);
                }

                if (missionProgress.HasValue)
                {
                    ev.EventParams.Add(EventParam.MissionProgress, missionProgress);
                }

                if (!string.IsNullOrEmpty(levelCollection1))
                {
                    ev.EventParams.Add(EventParam.LevelCollection1, levelCollection1);
                }

                if (!string.IsNullOrEmpty(levelCollection2))
                {
                    ev.EventParams.Add(EventParam.LevelCollection2, levelCollection2);
                }

                if (!string.IsNullOrEmpty(missionType))
                {
                    ev.EventParams.Add(EventParam.MissionType, missionType);
                }

                if (!string.IsNullOrEmpty(missionName))
                {
                    ev.EventParams.Add(EventParam.MissionName, missionName);
                }

                LogEvent(ev);
                HomaAnalytics.LevelFailed();
                await UniTask.Yield();
            });
            //PostLevelTime(levelAnalytics);
        }

        public static void LevelRestart(int level, int attemptNum, int? score = null,
            string levelCollection1 = null, string levelCollection2 = null,
            string missionType = null, string missionName = null)
        {
            UniTask.Void(async () =>
            {
                try
                {
                    var ev = new GtapGameEvent
                    {
                        GtapAnalyticsEventType = GtapAnalyticsEventType.Level,
                        EventType = EventType.Restart,
                        // Required params
                        EventParams = new Dictionary<string, object>
                        {
                            { EventParam.MissionID, level.ToString() },
                            { EventParam.MissionAttempt, attemptNum }
                        }
                    };

                    // Additional params
                    if (score.HasValue)
                    {
                        ev.EventParams.Add(EventParam.UserScore, score);
                    }

                    if (!string.IsNullOrEmpty(levelCollection1))
                    {
                        ev.EventParams.Add(EventParam.LevelCollection1, levelCollection1);
                    }

                    if (!string.IsNullOrEmpty(levelCollection2))
                    {
                        ev.EventParams.Add(EventParam.LevelCollection2, levelCollection2);
                    }

                    if (!string.IsNullOrEmpty(missionType))
                    {
                        ev.EventParams.Add(EventParam.MissionType, missionType);
                    }

                    if (!string.IsNullOrEmpty(missionName))
                    {
                        ev.EventParams.Add(EventParam.MissionName, missionName);
                    }

                    LogEvent(ev);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }

                await UniTask.Yield();
            });
        }

        public static void TutorialStepStart(int id)
        {
            UniTask.Void(async () =>
            {
                try
                {
                    var ev = new GtapGameEvent()
                    {
                        GtapAnalyticsEventType = GtapAnalyticsEventType.Tutorial,
                        EventParams = new Dictionary<string, object>
                        {
                            { EventParam.LevelStepName, id },
                        }
                    };

                    HomaAnalytics.TutorialStepStarted(id);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });
        }

        public static void TutorialStepComplete()
        {
            UniTask.Void(async () =>
            {
                try
                {
                    HomaAnalytics.TutorialStepCompleted();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });
        }

        public static void TutorialStepFailed()
        {
            UniTask.Void(async () =>
            {
                try
                {
                    HomaAnalytics.TutorialStepFailed();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });
        }

        public static void CustomEvent(string eventName)
        {
            UniTask.Void(async () =>
            {
                try
                {
                    Debug.LogWarning(eventName);
                    HomaAnalytics.DesignEvent(eventName);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });
        }

        public static void Upgrade(int level, string upgradeType)
        {
            UniTask.Void(async () =>
            {
                try
                {
                    var data = new DesignDimensions("Day_" + CurrentDay, upgradeType, score: level);
                    HomaAnalytics.DesignEvent(EventConstants.Upgrade, data);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });
        }

        public static void ResourceCollected(int level, int attemptsCount, EResourceCollected resourceCollected, int value)
        {
            if (Time.timeSinceLevelLoad < 5) return;

            UniTask.Void(async () =>
            {
                try
                {
                    var data = new DesignDimensions(level.ToString(), attemptsCount.ToString(), resourceCollected.ToString(), score: value);
                    HomaAnalytics.DesignEvent(EventConstants.ResourceCollected, data);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });
        }

        public static void BuildTile(string nameTile)
        {
            Debug.Log($"Build tile event: {nameTile}");
            UniTask.Void(async () =>
            {
                try
                {
                    var data = new DesignDimensions(nameTile, CurrentDay.ToString());
                    HomaAnalytics.DesignEvent(EventConstants.EventTileBuild, data);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });
        }

        public static void BuyNpc(string vendorName)
        {
            UniTask.Void(async () =>
            {
                try
                {
                    var data = new DesignDimensions("Day_" + CurrentDay, vendorName);
                    HomaAnalytics.DesignEvent(EventConstants.BuyNpc, data);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });
        }

        public static void CommitUnderwaterDeaths()
        {
            try
            {
                var data = new DesignDimensions("Day_" + CurrentDay, score: DeathInWater);
                HomaAnalytics.DesignEvent(EventConstants.UnderwaterDeaths, data);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void BuyWeapon(string weaponName)
        {
            UniTask.Void(async () =>
            {
                try
                {
                    var data = new DesignDimensions("Day_" + CurrentDay, weaponName);
                    HomaAnalytics.DesignEvent(EventConstants.BuyWeapon, data);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });
        }

        public static void TimeSpentInWater(int level, int attemptsCount, float timeSpent)
        {
            if (Time.timeSinceLevelLoad < 5) return;

            UniTask.Void(async () =>
            {
                try
                {
                    var data = new DesignDimensions(level.ToString(), attemptsCount.ToString(), score: timeSpent);
                    HomaAnalytics.DesignEvent(EventConstants.TimeSpentInWater, data);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });
        }

        public static void GdprShowEvent(string name, string country, bool consentDialogState)
        {
            UniTask.Void(async () =>
            {
                try
                {
                    var ev = new GtapGameEvent()
                    {
                        GtapAnalyticsEventType = GtapAnalyticsEventType.Custom,
                        EventParams = new Dictionary<string, object>
                        {
                            { "event_name", name },
                            { "country_code", country },
                            { "consent_dialog_state", consentDialogState == true ? "Applies" : "DoesNotApply" },
                        }
                    };

                    //GtapAnalytics.LogEvent(ev);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });
        }

        public static void LogEvent(GtapGameEvent ev)
        {
            UniTask.Void(async () =>
            {
                try
                {
                    CoreDebug.Log(JsonConvert.SerializeObject(ev.EventParams));
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                await UniTask.Yield();
            });
        }

        private static int _currentDay;

        public static int CurrentDay
        {
            get => _currentDay;
            set
            {
                _currentDay = value;
                DeathInWater = 0;
            }
        }
        
        public static int DeathInWater { get; set; }

        public class LevelAnalytics
        {
            private readonly string _formattedLevelNumber;

            private float _levelStartedDate;

            public string FormattedLevelName => _formattedLevelNumber;

            internal LevelAnalytics(string levelNumber)
            {
                LevelNumber = levelNumber;

                if (LevelNumber.All(char.IsDigit))
                {
                    _formattedLevelNumber = LevelNumber.PadLeft(4, '0');
                }
                else
                {
                    _formattedLevelNumber = LevelNumber;
                }

                _formattedLevelNumber = _formattedLevelNumber.Insert(0, "Level_");
            }

            public string LevelNumber { get; set; }

            public LevelAnalytics ProgressionStart()
            {
                _levelStartedDate = Time.unscaledTime;
                return this;
            }

            public float GetLevelTime()
            {
                return (float)(Time.unscaledTime - _levelStartedDate);
            }
        }
    }
}