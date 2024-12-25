using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.WindowManager
{
    public abstract class UIWindow : MonoBehaviour
    {
        [field: SerializeField] public Image backGround { get; private set; }
        [field: SerializeField] public RectTransform content { get; private set; }
        protected WindowManager WindowManager { get; private set; }

        public virtual void Initialize(WindowManager windowManager)
        {
            WindowManager = windowManager;
        }

        public virtual void Show()
        {
            if (backGround != null)
            {
                backGround.enabled = true;
            }

            if (content != null)
            {
                content.localScale = Vector3.one;
            }
        }

        public virtual void Hide()
        {
            if (backGround != null)
            {
                backGround.enabled = false;
            }

            if (content != null)
            {
                content.localScale = Vector3.zero;
            }
        }
    }
}
