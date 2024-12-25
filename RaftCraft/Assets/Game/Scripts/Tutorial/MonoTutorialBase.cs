using UnityEngine;
using System;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Saves;
using Game.Scripts.UI.WindowManager;

public abstract class MonoTutorialBase : MonoBehaviour
{
    public event Action OnComplete = () => { };

    public event Action<bool> OnChangeButton = _ => { };

    public abstract void Initialize(GameSave gameSave, IPlayerService playerSpawner, WindowManager windowManager, object extraParams = null);
    protected virtual void Complete()
    {
        OnComplete.Invoke();
    }

    protected void ShowBattleButton(bool value)
    {
        OnChangeButton.Invoke(value);
    }
}
