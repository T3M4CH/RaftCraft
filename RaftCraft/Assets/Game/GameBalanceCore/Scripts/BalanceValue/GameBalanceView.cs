using Game.GameBalanceCore.Scripts.BalanceValue.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.GameBalanceCore.Scripts.BalanceValue
{
    [CreateAssetMenu(menuName = "GameBalance/View", fileName = "GameBalanceView")]
    public class GameBalanceView : ScriptableObject
    {
        [SerializeField, FoldoutGroup("Settings")] private WindowBalanceGame _prefabWindow;

        private WindowBalanceGame _instance;

        public void Show()
        {
            if (_instance == null)
            {
                _instance = Instantiate(_prefabWindow);
            }
        
            _instance.gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (_instance == null)
            {
                return;
            }
        
            _instance.gameObject.SetActive(false);
        }
    }
}
