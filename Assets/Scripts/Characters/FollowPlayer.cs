using Cinemachine;
using UnityEngine;

namespace Characters
{
    public class FollowPlayer : MonoBehaviour
    {
        private CinemachineFreeLook _camera;
        private void Awake()
        {
            _camera = GetComponent<CinemachineFreeLook>();
        }

        public void Follow()
        {
            var followTarget = GameObject.FindWithTag("Player").transform;
            _camera.Follow = followTarget;
            _camera.LookAt =  followTarget;
        }
    }
}
