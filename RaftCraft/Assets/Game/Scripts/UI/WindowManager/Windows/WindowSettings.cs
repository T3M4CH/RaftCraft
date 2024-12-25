using GTapSoundManager.SoundManager;
using UnityEngine;
using TMPro;

namespace Game.Scripts.UI.WindowManager.Windows
{
    public class WindowSettings : UIWindow
    {
        [SerializeField] private TextMeshProUGUI _textVersion;
        [SerializeField] private SoundAsset _openSound;
        
        public override void Initialize(WindowManager windowManager)
        {
            base.Initialize(windowManager);
            _textVersion.text = $"Version {StaticBuildAnalysis.BundleVersion}";
        }

        public void CloseSettings()
        {
            WindowManager.CloseWindow<WindowSettings>();
        }

        public override void Show()
        {
            base.Show();

            _openSound.Play();
        }
        
        [field: SerializeField] public RectTransform ContentSettings { get; private set; }
    }
}
