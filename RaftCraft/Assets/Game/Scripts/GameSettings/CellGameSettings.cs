using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Toggle = Game.Scripts.UI.Elements.Toggle;

namespace Game.Scripts.GameSettings
{
    public class CellGameSettings : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("UI")] private Image _iconSettings;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textNameSettings;

        [field: SerializeField, FoldoutGroup("UI")]
        public Toggle Toggle { get; private set; }

        private Sprite _enabled;
        private Sprite _disabled;

        public void Init()
        {
            _iconSettings.enabled = false;
            _textNameSettings.enabled = false;
            Toggle.Setup(true);
        }

        public void Init(bool initState)
        {
            _iconSettings.enabled = false;
            _textNameSettings.enabled = false;
            Toggle.Setup(initState);
        }

        public void Init(bool initState, Sprite icon, Sprite iconDisable,  string nameSystem)
        {
            _enabled = icon;
            _disabled = iconDisable;
            _iconSettings.sprite = icon;
            _textNameSettings.text = nameSystem;
            Toggle.Setup(initState);
            ToggleOnStateChanged(initState);
            Toggle.StateChanged += ToggleOnStateChanged;
        }

        private void ToggleOnStateChanged(bool state)
        {
            _iconSettings.sprite = state ? _enabled : _disabled;
        }

        private void OnDestroy()
        {
            Toggle.StateChanged -= ToggleOnStateChanged;
        }
    }
}
