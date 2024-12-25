using Game.Scripts.UI.DayProgress.Enums;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ColumnDayInfo : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image longColumn;
    [SerializeField] private Image shortColumn;
    [SerializeField] private GameObject shortColumnHandler;

    [SerializeField, FoldoutGroup("Icons")] private Sprite weaponIcon;
    [SerializeField, FoldoutGroup("Icons")] private Sprite playerUpgradeNpc;
    [SerializeField, FoldoutGroup("Icons")] private Sprite aqualang;
    [SerializeField, FoldoutGroup("Icons")] private Sprite weaponUpgrade;
    [SerializeField, FoldoutGroup("Icons")] private Sprite fishUpgrade;
    [SerializeField, FoldoutGroup("Icons")] private Sprite logNpcIcon;
    [SerializeField, FoldoutGroup("Icons")] private Sprite diceIcon;
    [SerializeField, FoldoutGroup("Icons")] private Sprite bossIcon;

    [SerializeField, FoldoutGroup("Colors")] private Color notStartedColor;
    [SerializeField, FoldoutGroup("Colors")] private Color inProgressColor;
    [SerializeField, FoldoutGroup("Colors")] private Color completeColor;
    [SerializeField, FoldoutGroup("Colors")] private Color bossColor;
    [SerializeField, FoldoutGroup("Colors")] private Color whiteIconColor;

    private EDayType _currentDayType;
    private GameObject _longColumnHandler;

    [Button]
    public void SetColumn(EDayType dayType)
    {
        icon.color = whiteIconColor;

        switch (dayType)
        {
            case EDayType.Default:
                ChangeActiveColumn(false);
                break;
            case EDayType.Weapon:
                ChangeActiveColumn(true);
                icon.sprite = weaponIcon;
                break;
            case EDayType.Dice:
                ChangeActiveColumn(true);
                icon.sprite = diceIcon;
                break;
            case EDayType.Boss:
                ChangeActiveColumn(true);
                icon.sprite = bossIcon;
                break;
            case EDayType.LogNpc:
                ChangeActiveColumn(true);
                icon.sprite = logNpcIcon;
                break;
            case EDayType.PlayerUpgradeNpc:
                ChangeActiveColumn(true);
                icon.sprite = playerUpgradeNpc;
                break;
            case EDayType.Aqualang:
                ChangeActiveColumn(true);
                icon.sprite = aqualang;
                break;
            case EDayType.WeaponUpgradeNpc:
                ChangeActiveColumn(true);
                icon.sprite = weaponUpgrade;
                break;
            case EDayType.FishUpgrade:
                ChangeActiveColumn(true);
                icon.sprite = fishUpgrade;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dayType), dayType, null);
        }

        _currentDayType = dayType;
    }

    [Button]
    public void SetColor(EDayCompleteType completeType)
    {
        switch (completeType)
        {
            case EDayCompleteType.NotStarted:
                ChangeColor(notStartedColor, _currentDayType == EDayType.Boss ? bossColor : null);
                break;
            case EDayCompleteType.InProgress:
                var color = _currentDayType == EDayType.Boss ? bossColor : inProgressColor;
                ChangeColor(color, color);
                break;
            case EDayCompleteType.Complete:
                ChangeColor(completeColor, whiteIconColor);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(completeType), completeType, null);
        }
    }

    private void ChangeColor(Color color, Color? iconColor = null)
    {
        icon.color = whiteIconColor;

        if (iconColor != null)
        {
            icon.color = iconColor.Value;
        }

        longColumn.color = color;
        shortColumn.color = color;
    }

    private void ChangeActiveColumn(bool isNotEmptyColumn)
    {
        shortColumnHandler.SetActive(isNotEmptyColumn);
        _longColumnHandler.SetActive(!isNotEmptyColumn);
    }

    private void Awake()
    {
        _longColumnHandler = longColumn.gameObject;
    }
}