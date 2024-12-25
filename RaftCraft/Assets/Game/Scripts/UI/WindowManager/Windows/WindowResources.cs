using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.Core.Interface;
using Game.Scripts.ResourceController;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.UI;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.WindowManager.Windows
{
    public class WindowResources : UIWindow, IGameObserver<ResourceItem>
    {
        [SerializeField, FoldoutGroup("UI")] private RectTransform _contentGlobal;
        [SerializeField, FoldoutGroup("Settings")] private ResourcesCell _prefabCellGlobal;
        [SerializeField, FoldoutGroup("Settings")] private ResourcesCell _prefabCellLocal;
        
        private IGameObservable<ResourceItem> _observable;

        private Dictionary<EResourceType, ResourcesCell> _poolCell = new Dictionary<EResourceType, ResourcesCell>();

        public RectTransform FishCell { get; private set; }
    
        [Inject]
        public void Init(IGameObservable<ResourceItem> observable)
        {
            _observable = observable;
            _observable.AddObserver(this);
            Show();
        }
        
        
        public void PerformNotify(ResourceItem data)
        {
            if (_poolCell.ContainsKey(data.Type) == false)
            {
                var cell = Instantiate(data.HaveGlobal ? _prefabCellGlobal : _prefabCellLocal, _contentGlobal);
                cell.InitIcon(data.Type);
                if (data.Type == EResourceType.UniversalFish)
                {
                    FishCell = cell.Icon.rectTransform;
                }
                _poolCell.Add(data.Type, cell);
                cell.gameObject.SetActive(data.Count > 0 || data.TempCount > 0);
            }
        
            _poolCell[data.Type].Notify(data);
            content.gameObject.SetActive(false);
            content.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentGlobal);
            Canvas.ForceUpdateCanvases();
        }
        

        private void Start()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentGlobal);
            Canvas.ForceUpdateCanvases();
        }

        public override void Hide()
        {
            base.Hide();

            gameObject.SetActive(false);
        }

        public override void Show()
        {
            base.Show();
        
            gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            if (_observable != null)
            {
                _observable.RemoveObserver(this);
            }
        }


    }
}
