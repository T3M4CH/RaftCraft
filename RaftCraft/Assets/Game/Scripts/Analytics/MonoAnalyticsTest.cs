using System.Collections;
using System.Collections.Generic;
using GTap.Analytics;
using UnityEngine;

public class MonoAnalyticsTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GtapAnalytics.LevelStart(0, 0, 1);
    }

}
