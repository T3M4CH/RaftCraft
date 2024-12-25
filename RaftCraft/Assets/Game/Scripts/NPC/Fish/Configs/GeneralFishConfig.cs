using System.Linq;
using Game.Scripts.Core;
using Game.Scripts.NPC.Fish;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "FishSettings", menuName = "Fish/GeneralFishSettings")]
public class GeneralFishConfig : ScriptableObject, IWindowObject
{
    [field: SerializeField, AssetsOnly] public BaseFish FishPrefab { get; private set; }
    [field: SerializeField] public FishData[] FishData { get; private set; }

    [GUIColor(0.55f, 1f, 0.42f), Button(ButtonSizes.Large)]
    private void Sort()
    {
        FishData = FishData.OrderBy(data => data.Level).ToArray();
    }
    
    public string Patch => "Fish/GeneralFishSettings";
    public object InstanceObject => this;
    
    public void CreateAsset()
    {
    }
}
