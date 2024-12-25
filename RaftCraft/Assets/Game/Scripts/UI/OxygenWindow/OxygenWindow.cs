using Game.Scripts.UI.WindowManager;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Player.Oxygen;
using Game.Scripts.Player;
using Reflex.Attributes;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class OxygenWindow : UIWindow
{
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private TMP_Text oxygenLeftText;
    [SerializeField] private Image clockFillAmount;
    [SerializeField] private Image sliderFillAmount;
    [SerializeField] private Image clockArrowImage;
    [SerializeField] private Image contur;
    [SerializeField] private TMP_Text _oxygenText;
    [SerializeField] private RectTransform clockArrow;
    [SerializeField] private CanvasGroup _canvasGroup;

    private bool _isActive;
    private float _currentTime;
    private Color _baseArrowColor;
    private Color _baseClockFillColor;
    private IPlayerService _playerService;
    private OxygenController _oxygenController;

    [Inject]
    private void Construct(IPlayerService playerService)
    {
        _playerService = playerService;

        _playerService.AddListener(ValidateWindowState);

        _baseArrowColor = clockArrowImage.color;
        _baseClockFillColor = clockFillAmount.color;
    }
    
    public override void Show()
    {
        base.Show();

        _canvasGroup.alpha = 1;
        _isActive = true;
    }

    public override void Hide()
    {
        base.Hide();

        _canvasGroup.alpha = 0;
        _isActive = false;
    }

    private void FixedUpdate()
    {
        if (_isActive)
        {
            var ratio = _oxygenController.Ratio;
            
            if(float.IsNaN(ratio)) return;
            clockFillAmount.fillAmount = ratio;
            sliderFillAmount.fillAmount = ratio;
            clockArrow.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(-360, 0, ratio));

            var oxygenValue = _oxygenController.CurrentOxygenValue;
            oxygenLeftText.text = $"{Mathf.Max(0, Mathf.RoundToInt(oxygenValue))}s";

            if (ratio <= 0.3f)
            {
                _currentTime -= Time.deltaTime;

                clockArrow.localScale = Vector3.one * (Mathf.PingPong(_currentTime / (delay * 2f), 0.25f) + 1);
                _oxygenText.rectTransform.localScale = Vector3.one * (Mathf.PingPong(_currentTime, 0.25f) + 1f);
                
                if (_currentTime < 0)
                {
                    _currentTime = delay;
                    
                    oxygenLeftText.color = oxygenLeftText.color == new Color(1f, 0.24f, 0.22f) ? new Color(0.97f, 1f, 1f) : new Color(1f, 0.24f, 0.22f);
                    clockArrowImage.color = clockArrowImage.color == new Color(1f, 0.24f, 0.22f) ? _baseArrowColor : new Color(1f, 0.24f, 0.22f);
                    contur.color = contur.color == new Color(1f, 0.24f, 0.22f) ? new Color(0.97f, 1f, 1f) : new Color(1f, 0.24f, 0.22f);
                    _oxygenText.color = _oxygenText.color == new Color(1f, 0.24f, 0.22f) ? new Color(0.97f, 1f, 1f) : new Color(1f, 0.24f, 0.22f);
                }
            }
            else
            {
                _oxygenText.color = new Color(0.97f, 1f, 1f);
                contur.color = new Color(0.97f, 1f, 1f);
                oxygenLeftText.color = new Color(0.97f, 1f, 1f);
                clockArrowImage.color = _baseArrowColor;
                
                clockArrow.localScale = Vector3.one;
                _oxygenText.rectTransform.localScale = Vector3.one;
            }
        }
    }

    private void ValidateWindowState(EPlayerStates state, EntityPlayer player)
    {
        _oxygenController = player.OxygenController;

        if (state == EPlayerStates.PlayerInWater)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void OnDestroy()
    {
        _playerService.RemoveListener(ValidateWindowState);

        _isActive = false;
    }
}