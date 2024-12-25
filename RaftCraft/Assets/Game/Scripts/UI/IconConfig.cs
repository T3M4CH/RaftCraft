using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core;
using Game.Scripts.Player.WeaponController;
using Game.Scripts.ResourceController.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "IconConfig", menuName = "Icons/IconConfig")]
public class IconConfig : ScriptableObject, IWindowObject
{
    [System.Serializable]
    public class ItemIcon<T> where T : Enum
    {
        [field: SerializeField] public T Type { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        public ItemIcon(T type)
        {
            Type = type;
            Icon = null;
        }
    }
    public string Patch => "Icons/IconConfig";
    public object InstanceObject => this;
    
    public void CreateAsset()
    {
    }

    [field: SerializeField, FoldoutGroup("Item Icons")]
    private List<ItemIcon<EResourceType>> _itemIcons = new List<ItemIcon<EResourceType>>();
    
    [field: SerializeField, FoldoutGroup("Item Icons")]
    private List<ItemIcon<EResourceType>> _itemIconsUi = new List<ItemIcon<EResourceType>>();

    [field: SerializeField, FoldoutGroup("Weapon Icons")]
    private List<ItemIcon<WeaponId>> _weaponIcons = new List<ItemIcon<WeaponId>>();
    [field: SerializeField] public Sprite HomeIcon { get; private set; }
    [field: SerializeField] public Sprite TombIcon { get; private set; }
    [field: SerializeField] public Sprite OxygenIcon { get; private set; }
    [field: SerializeField] public Sprite MaxDepthIcon { get; private set; }
    [field: SerializeField] public Sprite WaterSpeedIcon { get; private set; }
    [field: SerializeField] public Sprite FishLevel { get; private set; }
    [field: SerializeField] public Sprite HammerIcon { get; private set; }
    [field: SerializeField] public Sprite WoodIcon { get; private set; }
    

    public Sprite GetIconItem(EResourceType resourceType)
    {
        var result = GetIconItemUI(resourceType);
        if (result != null)
        {
            return result;
        }
        return (from icon in _itemIcons where icon.Type == resourceType select icon.Icon).FirstOrDefault();
    }

    private Sprite GetIconItemUI(EResourceType resourceType)
    {
        return (from icon in _itemIconsUi where icon.Type == resourceType select icon.Icon).FirstOrDefault();
    }

    public Sprite GetIconWeapon(WeaponId weaponId)
    {
        return (from icon in _weaponIcons where icon.Type == weaponId select icon.Icon).FirstOrDefault();
    }

    [Button]
    private void Generate()
    {
        _itemIcons.Clear();
        foreach (var resource in (EResourceType[]) Enum.GetValues(typeof(EResourceType)))
        {
            _itemIcons.Add(new ItemIcon<EResourceType>(resource));
        }
    }
}
