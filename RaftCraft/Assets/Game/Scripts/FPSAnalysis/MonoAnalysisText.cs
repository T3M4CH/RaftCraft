using System.Globalization;
using TMPro;
using UnityEngine;

public class MonoAnalysisText : MonoBehaviour
{
    [SerializeField] private TMP_Text sinceStartup;
    [SerializeField] private TMP_Text bundleVersion;
    [SerializeField] private TMP_Text totalBefore;
    [SerializeField] private TMP_Text totalAfter;
    
    private void Start()
    {
        sinceStartup.text = $"Time start {StaticBuildAnalysis.TimeSinceStartup:F}";
        bundleVersion.text = $"Version {StaticBuildAnalysis.BundleVersion}";
        totalBefore.text = $"Total : {(StaticBuildAnalysis.BeforeSceneLoadDateTime - StaticBuildAnalysis.ApplicationStartDateTime).TotalSeconds:F}";
        totalAfter.text = $"Total After : {(StaticBuildAnalysis.AfterSceneLoadDateTime - StaticBuildAnalysis.ApplicationStartDateTime).TotalSeconds:F}";
    }
}
