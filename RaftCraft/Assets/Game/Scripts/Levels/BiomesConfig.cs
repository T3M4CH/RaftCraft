using Game.Scripts.Levels.Enums;
using Game.Scripts.Core;
using UnityEngine;
using System;
using System.Collections.Generic;
using Game.Scripts.UI.DayProgress.Enums;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Biomes", fileName = "BiomesConfig")]
public class BiomesConfig : ScriptableObject, IWindowObject
{
    public string Patch => "Biomes/BiomesConfig";
    public object InstanceObject => this;

    public SerializableLevelStruct[] Levels;

    public void CreateAsset()
    {
    }
}

[Serializable]
public struct SerializableLevelStruct
{
    [field: SerializeField, ReadOnly] public int DayId { get; set; }
    [field: SerializeField] public EDayType DayType { get; private set; }
    [field: SerializeField] public EBiomeType BiomeType { get; private set; }
    [field: SerializeField] public SerializableScene Scene { get; private set; }
}