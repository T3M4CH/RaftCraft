using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.WindowManager.Windows
{
    public class WindowShopWood : UIWindow
    {
        [SerializeField] private RectTransform _contenPrice;
        private void Awake()
        {
            Hide();
        }

        public override void Show()
        {
            backGround.enabled = true;
            content.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contenPrice);
            Canvas.ForceUpdateCanvases();
        }

        public override void Hide()
        {
            backGround.enabled = false;
            content.gameObject.SetActive(false);
        }
    }
}
