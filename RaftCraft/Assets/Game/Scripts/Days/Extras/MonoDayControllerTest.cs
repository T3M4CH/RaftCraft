using Game.Scripts.Days;
using GTapSoundManager.SoundManager;
using UnityEngine.InputSystem;
using Reflex.Attributes;
using UnityEngine;

public class MonoDayControllerTest : MonoBehaviour
{
    [SerializeField] private SoundAsset chpokAsset;
    
    private IDayService _dayController;
    
    [Inject]
    private void Construct(IDayService dayController)
    {
        _dayController = dayController;
        
        // chpokAsset.Initialize();
    }

    private void Update()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            _dayController.CompleteDay();
        }

        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            chpokAsset.Play();
        }
    }
}
