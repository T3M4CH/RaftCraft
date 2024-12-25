using UnityEngine;
using UnityEngine.UI;

namespace Game.GameBalanceCore.Scripts.BalanceValue
{
    public class BalanceFloat : BalanceValue<float>
    {
        private float _value;

        private float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, _minValue, _maxValue);
                Notify(_value);
            }
        }

        public BalanceFloat(float min, float max, string nameValue, float defaultValue) : base(min, max, nameValue)
        {
            Value = defaultValue;
            Debug.Log($"Init value: {Value}:{defaultValue}");
        }

        public override void InitSlider(Slider slider)
        {
            slider.minValue = _minValue;
            slider.maxValue = _maxValue;
            slider.value = Value;
        }

        public override void SetValue(float value)
        {
            Value = value;
        }

        public override float GetValue(float defaultValue)
        {
            return defaultValue + (defaultValue * (Value / 100f));
        }

        public override float GetCurrent()
        {
            return Value;
        }

        public override string ToString()
        {
            return $"{_nameValue}{Value:0.0}";
        }
    }
}