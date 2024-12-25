using Game.Scripts.Saves;
using Game.Scripts.UI.WindowManager;
using Reflex.Attributes;

namespace Game.Scripts.Tutorial.TutorialBuildVendor
{
    public class TutorialBuildVendorWeapon : TutorialBuildVendorBase
    {
        private WindowManager _windowManager;
        
        [Inject]
        private void Construct(WindowManager windowManager)
        {
            _windowManager = windowManager;
        }
        
        public override void StartTutorial()
        {
            base.StartTutorial();
            _windowManager.SetNameTutorial("Buy vendor");
        }

        public override void Complete(GameSave gameSave)
        {
            base.Complete(gameSave);
            _windowManager.SetNameTutorial("");
        }

        protected override string NameShop()
        {
            return "ShopControllerWeapon";
        }
    }
}