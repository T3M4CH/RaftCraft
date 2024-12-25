using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Scripts.CameraSystem.Cameras
{
    public class PlayerVirtualCamera : BaseVirtualCamera
    {
        private CinemachineComposer _composer;
        private CinemachineTransposer _transposer;
        
        public async UniTaskVoid ResetDamping(Vector3 bodyDamping, Vector2 aimDamping, float returnDelay)
        {
            var transposerValues = new Vector3(_transposer.m_XDamping, _transposer.m_YDamping, _transposer.m_ZDamping);
            var composerValues = new Vector2(_composer.m_HorizontalDamping, _composer.m_VerticalDamping);

            _transposer.m_XDamping = bodyDamping.x;
            _transposer.m_YDamping = bodyDamping.y;
            _transposer.m_ZDamping = bodyDamping.z;

            _composer.m_VerticalDamping = aimDamping.y;
            _composer.m_HorizontalDamping = aimDamping.x;

            await UniTask.Delay(TimeSpan.FromSeconds(returnDelay), cancellationToken: this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

            _transposer.m_XDamping = transposerValues.x;
            _transposer.m_YDamping = transposerValues.y;
            _transposer.m_ZDamping = transposerValues.z;

            _composer.m_VerticalDamping = composerValues.y;
            _composer.m_HorizontalDamping = composerValues.x;
        }
        
        private void Awake()
        {
            _transposer = Camera.GetCinemachineComponent<CinemachineTransposer>();
            _composer = Camera.GetCinemachineComponent<CinemachineComposer>();
        }
    }
}