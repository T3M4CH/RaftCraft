using Game.Scripts.Core;
using Game.Scripts.NPC.Fish.Configs.Structs;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.NPC.Fish.Configs
{
    [CreateAssetMenu(fileName = "FishInBiome", menuName = "Fish/FishInBiome")]
    public class FishInBiomes : ScriptableObject, IWindowObject
    {
        [SerializeField] private TextAsset textAsset;
        [field: SerializeField] public GeneralFishConfig FishConfig { get; private set; }
        
        [field: SerializeField] public SerializableFishPerDay[] FishPerDays { get; private set; }
        
        public string Patch => $"Fish/{name}";
        public object InstanceObject => this;
        public void CreateAsset()
        {
        }

        [Button]
        private void Serialize()
        {
            FishPerDays = JsonConvert.DeserializeObject<SerializableFishPerDay[]>(textAsset.text);
        }
    }
}