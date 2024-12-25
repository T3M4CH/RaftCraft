using System;
using DG.Tweening;
using Game.Scripts.CameraSystem.Cameras;
using Game.Scripts.CameraSystem.Interfaces;
using UnityEngine;

namespace Game.Scripts.BattleMode
{
    public class CutScene
    {
        private readonly ICameraService _cameraService;
        private readonly Material _waterMaterial;
        private readonly FocusVirtualCamera _focusCam;
        
        private int _hashDarken = Shader.PropertyToID("_Darken"); 

        private Sequence _sequence;

        public CutScene(ICameraService cameraService)
        {
            _cameraService = cameraService;
            _waterMaterial = Resources.Load<Material>("WaterBackground");
            _focusCam = cameraService.GetCamera<FocusVirtualCamera>();
        }

        public void PlayScene(Transform target, float duration, Action callback, float delay)
        {
            _sequence = DOTween.Sequence()
                .AppendInterval(delay)
                .AppendCallback(() =>
                {
                    _cameraService.ChangeActiveCamera<FocusVirtualCamera>();
                    
                    _focusCam.SetFollowAt(target)
                        .SetLookAt(target);
                })
                .AppendInterval(duration)
                .AppendCallback(() =>
                {
                    _cameraService.ChangeActiveCamera<PlayerVirtualCamera>();
                    callback.Invoke();
                });
        }

        public void WaterChangeColor(bool darkenWater, float duration)
        {
            var darken = darkenWater ? 0f : 1f;

            DOTween
                .To(() => darken, x => darken = x, darkenWater ? 1f : 0f, duration)
                .OnUpdate(() =>
                {
                    _waterMaterial.SetFloat(_hashDarken, darken);
                });
        }
        
        public void ResetCutscene()
        {
            _waterMaterial.SetFloat(_hashDarken, 0f);
            _sequence.Kill();
        }
    }
}