using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Raft.Components
{
    public class TileUIController : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("UI")] private Transform _panelUI;
        [SerializeField, FoldoutGroup("UI")] private Transform _panelBig;
        
        [SerializeField, FoldoutGroup("Components")]
        private BoxCollider _collider;

        [SerializeField, FoldoutGroup("Settings")]
        private Vector2 _minMax;

        public void SetBigPanelState(bool state)
        {
            if(_panelBig)
                _panelBig.gameObject.SetActive(state);    
        }
        
        public void SetPosition(Vector3 positionBuild)
        {
            _panelUI.localPosition = positionBuild;
            _collider.center = positionBuild;
        }
    }
}
