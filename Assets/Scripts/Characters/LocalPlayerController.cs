using Cinemachine;
using Network;
using Network.Models.Other;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters
{
    public class LocalPlayerController : MonoBehaviour
    {
        [FormerlySerializedAs("CharacterController")]
        public CharacterController characterController;

        public float playerSpeed = 10f;
        public float cameraSpeed = 2f;
        public GameObject followTarget;
        private GameObject _networkControllerGameObject;
        private NetworkController _networkController;
        private Vector3 _lastPosition;
        private float _sendTimer = 0f;
        
        private void Start()
        {
            _networkControllerGameObject = GameObject.Find("NetworkController");
            _networkController = _networkControllerGameObject.GetComponent<NetworkController>();
            FindObjectOfType<CinemachineVirtualCamera>().GetComponent<FollowPlayer>().Follow();
        }

        private void Update()
        {
            RotateCamera();
        }

        private void FixedUpdate()
        {           
            Move();

            if (_lastPosition == transform.position) return;

            var position1 = transform.position;
            _lastPosition = position1;

            var position = new Position
            {
                rotation = transform.eulerAngles.y,
                x = position1.x,
                y = position1.y,
                z = position1.z
            };
            if (_sendTimer.Equals(1f))
            {
                _networkController.OnLocalPlayerMove(position);
                _sendTimer = 0f;
            }
            else
            {
                _sendTimer++;
            }
        }
        
        private void Move()
        {
            var horizontalMove = Input.GetAxis("Horizontal");
            var verticalMove = Input.GetAxis("Vertical");

            if (verticalMove != 0f || horizontalMove != 0f)
            {
                Rotate();
            }

            var move = transform.forward * verticalMove + transform.right * horizontalMove;
            characterController.Move(playerSpeed * Time.deltaTime * move);
        }
     
        private void RotateCamera()
        {
            var mouseX = cameraSpeed * Input.GetAxis("Mouse X");
            var mouseY = cameraSpeed * Input.GetAxis("Mouse Y");
            followTarget.transform.Rotate(mouseY, mouseX, 0);
            var z = followTarget.transform.eulerAngles.z;
            followTarget.transform.Rotate(0, 0, -z);
        }

        private void Rotate()
        {
            var lastRotationFollowTarget = followTarget.transform.rotation;
            if (transform.rotation.y != followTarget.transform.rotation.y)
            {
                var eulerRotation = new Vector3(transform.eulerAngles.x, followTarget.transform.eulerAngles.y,
                    transform.eulerAngles.z);
                transform.rotation = Quaternion.Euler(eulerRotation);
                followTarget.transform.rotation = lastRotationFollowTarget;
            }
        }
    }
}