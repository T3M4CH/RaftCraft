using System;
using DG.Tweening;
using Game.Scripts.Player;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.PlayerController.PlayerStates;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Player.StateMachine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Image _frontLine;
    [SerializeField] private Image _backLine;
    [SerializeField] private CanvasGroup healthBar;
    [SerializeField] private TextMeshProUGUI _textValue;
    [SerializeField] private CanvasGroup _groupText;
    [SerializeField] private float _durationMove;
    [SerializeField] private Ease _easeMove;

    [SerializeField, FoldoutGroup("Feedback Text")]
    private float _delayText;

    [SerializeField, FoldoutGroup("Feedback Text")]
    private float _durationText;

    [SerializeField, FoldoutGroup("Feedback Text")]
    private Ease _easeText;

    [SerializeField] private EntityPlayer _entityPlayer;

    private bool isOnPlot = true;

    public void SetValue(float value)
    {
        _frontLine.fillAmount = value;
        _backLine.DOKill();
        _backLine.DOFillAmount(value, _durationMove).SetEase(_easeMove);

        if (value <= 0)
        {
            healthBar.alpha = 0;
            gameObject.SetActive(false);
        }
    }

    public void SetValueTextHeals(int heals)
    {
        _groupText.DOKill();
        _groupText.alpha = 1f;
        healthBar.alpha = 1f;
        _textValue.text = $"{heals}";
        _groupText.DOFade(0f, _durationText).SetEase(_easeText).SetDelay(_delayText);

        if (isOnPlot) return;
        
        healthBar.DOKill();
        healthBar.DOFade(0, _durationText).SetEase(_easeText).SetDelay(_delayText);
    }

    private void ValidatePlayer(EntityState state, Entity _)
    {
        if (state is PlayerPlotState)
        {
            isOnPlot = true;
            healthBar.alpha = 1;
        }
        else
        {
            isOnPlot = false;
            healthBar.DOKill();
            healthBar.DOFade(0, _durationText).SetEase(_easeText).SetDelay(_delayText);
        }
    }

    private void Start()
    {
        //TODO: Какого то на всех ентити весит
        if (_entityPlayer)
        {
            _entityPlayer.StateMachine.OnEnterState += ValidatePlayer;
            isOnPlot = true;
            healthBar.alpha = 1;
        }
    }

    private void OnDestroy()
    {
        healthBar.DOKill();
        _groupText.DOKill();
    }
}