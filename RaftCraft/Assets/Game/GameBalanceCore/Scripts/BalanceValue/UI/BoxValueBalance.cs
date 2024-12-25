using System;
using Game.Scripts.Core.Interface;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameBalanceCore.Scripts.BalanceValue.UI
{
    public class BoxValueBalance : MonoBehaviour, IGameObserver<float>
    {
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textValue;
        [SerializeField, FoldoutGroup("UI")] private Slider _sliderValue;
        [SerializeField, FoldoutGroup("UI")] private TMP_InputField _inputField;
        private BalanceFloat _balanceFloat;
    
        public void Init(BalanceFloat balanceFloat)
        {
            _balanceFloat = balanceFloat;
            _balanceFloat.AddObserver(this);
            _textValue.text = _balanceFloat.GetName();
            _inputField.text = $"{_balanceFloat.GetCurrent():0}%";
        }

        private void Start()
        {
            _balanceFloat.InitSlider(_sliderValue);
            _sliderValue.onValueChanged.AddListener(OnChangeSlider);
            _inputField.onValueChanged.AddListener(OnChangeText);
        }
        
        private void OnChangeText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = text.TrimEnd('%');
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }
                var value = float.Parse(text);
                if (_balanceFloat != null)
                {
                    _balanceFloat.SetValue(value);
                }
            }
        }

        private void OnChangeSlider(float value)
        {
            if (_balanceFloat != null)
            {
                _balanceFloat.SetValue(value);
            }
        }

        public void PerformNotify(float data)
        {
            _inputField.text = $"{data:0}%";
            _sliderValue.value = data;
        }

        private void OnDestroy()
        {
            _sliderValue.onValueChanged.RemoveListener(OnChangeSlider);
            _balanceFloat.RemoveObserver(this);
            _inputField.onValueChanged.RemoveListener(OnChangeText);
        }
    }
}
