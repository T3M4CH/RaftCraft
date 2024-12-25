using UnityEngine;

namespace Game.Scripts.Utils
{
    public class FollowCamera : MonoBehaviour
    {
        private Camera _camera;


        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            var rotation = _camera.transform.rotation;
            transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
        }
    }
}
