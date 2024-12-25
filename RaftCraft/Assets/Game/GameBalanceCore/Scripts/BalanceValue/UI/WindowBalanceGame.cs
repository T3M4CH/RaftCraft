using Game.GameBalanceCore.scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.GameBalanceCore.Scripts.BalanceValue.UI
{
    public class WindowBalanceGame : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Settings")] private BoxValueBalance _prefabValue;
        [SerializeField, FoldoutGroup("Settings")] private RectTransform _content;
        
        private void Awake()
        {
            var balances = GameBalance.Instance.GetBalances();
            foreach (var balance in balances)
            {
                var cell = Instantiate(_prefabValue, _content);
                cell.Init(balance.Value);
            }
        }
    }
}
