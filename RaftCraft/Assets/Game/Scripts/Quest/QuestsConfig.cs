using Game.Scripts.Quest.Structs;
using Game.Scripts.Core;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestsPerDay", menuName = "Quests/QuestsPerDay")]
public class QuestsConfig : ScriptableObject, IWindowObject
{
    [field: SerializeField] public SerializableQuestPerDay[] QuestPerDays { get; private set; }
    
    public void CreateAsset()
    {
    }

    public string Patch => $"Quests/{name}";
}