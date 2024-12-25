using System;
using Game.Scripts.NPC.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Health
{
    public class MonoHealthBar : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Image levelBackImage;
        [SerializeField] private RectTransform levelPanel;

        private IHeals _heals;

        public void SetTarget(IHeals heals)
        {
            _heals = heals;
            heals.OnUpdateHealsProgress += HealsOnOnUpdateHealsProgress;
        }

        private void HealsOnOnUpdateHealsProgress(float progress)
        {
            FillElement.fillAmount = progress;
        }

        public void SetLevel(int value, Color color)
        {
            Level = value;

            levelBackImage.color = color;
            levelText.text = Level.ToString();
            levelPanel.localScale = Vector3.one;
            Canvas.ForceUpdateCanvases();
        }

        private void OnDestroy()
        {
            if (_heals == null)
            {
                return;
            }
            _heals.OnUpdateHealsProgress -= HealsOnOnUpdateHealsProgress;
        }

        public int Level { get; private set; }
        [field: SerializeField] public Image FillElement { get; private set; }
        [field: SerializeField] public RectTransform RectTransform { get; private set; }
    }
}