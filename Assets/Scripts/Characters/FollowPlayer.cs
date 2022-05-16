using Cinemachine;
using UnityEngine;

namespace Characters
{
    public class FollowPlayer : MonoBehaviour
    {
        private CinemachineVirtualCamera _camera;
        private void Awake()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        public void Follow()
        {
            var followTarget = GameObject.FindWithTag("LookAt").transform;
            _camera.Follow = followTarget;
        }
    }
}
