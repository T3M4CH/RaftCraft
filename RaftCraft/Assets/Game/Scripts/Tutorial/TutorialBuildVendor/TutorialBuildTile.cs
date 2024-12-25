using Game.Scripts.Raft.BuildSystem;
using Game.Scripts.UI.WindowManager;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Scripts.Tutorial.TutorialBuildVendor
{
    public class TutorialBuildTile : TutorialBuildVendor
    {
        [SerializeField] private TileBuild _targetTile;
        
        private bool _showArrow;
        private WindowManager _windowManager;
        
        [Inject]
        private void Construct(WindowManager windowManager)
        {
            _windowManager = windowManager;
        }
        
        public override int Cost()
        {
            if (_targetTile == null)
            {
                return int.MaxValue;
            }

            return _targetTile.CountResource;
        }
        
        public override void StartTutorial()
        {
            if (_targetTile == null)
            {
                return;
            }

            if (_targetTile.HaveConstruct)
            {
                OnComplete?.Invoke(this);
                return;
            }
            _windowManager.SetNameTutorial("Build Raft");
            _showArrow = true;
            _targetTile.OnBuyTile += OnBuyTile;
            _targetTile.OnStartBuilding += OnStartBuilding;
        }

        private void OnStartBuilding()
        {
            _showArrow = false;
        }

        public override bool ShowArrow()
        {
            return _showArrow;
        }

        private void OnBuyTile()
        {
            _targetTile.OnStartBuilding -= OnStartBuilding;
            _targetTile.OnBuyTile -= OnBuyTile;
            _windowManager.SetNameTutorial("");
            OnComplete?.Invoke(this);
        }

        public override Transform Target()
        {
            if (_targetTile == null)
            {
                return transform;
            }
            transform.position = _targetTile.PositionUI;
            return transform;
        }

        private void OnDestroy()
        {
            if (_targetTile != null)
            {
                _targetTile.OnStartBuilding -= OnStartBuilding;
                _targetTile.OnBuyTile -= OnBuyTile;
            }
        }
    }
}
