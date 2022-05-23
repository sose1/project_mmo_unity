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
        public string animationState = "IDLE";

        private GameObject _networkControllerGameObject;
        private NetworkController _networkController;
        private Vector3 _lastPosition;
        private float _sendTimer = 0f;
        private Vector3 gravityMove;
        private bool isGrounded;
        private int idleCountSend = 0;
        
        private void Start()
        {
            _networkControllerGameObject = GameObject.Find("NetworkController");
            _networkController = _networkControllerGameObject.GetComponent<NetworkController>();
            FindObjectOfType<CinemachineVirtualCamera>().GetComponent<FollowPlayer>().Follow();
        }

        private void Update()
        {
            if (!characterController.isGrounded)
                gravityMove.y += Physics.gravity.y * Time.deltaTime * 0.06f;
            else
                gravityMove.y = 0f;

            RotateCamera();
        }

        private void FixedUpdate()
        {
            Move();
            var position1 = transform.position;
            var position = new Position
            {
                rotation = transform.eulerAngles.y,
                x = position1.x,
                y = position1.y,
                z = position1.z
            };
            
            if (_lastPosition == transform.position)
            {
                if (idleCountSend <= 5)
                {
                    if (idleCountSend == 5)
                        idleCountSend = 5;
                    _networkController.OnLocalPlayerMove(position, animationState);
                    idleCountSend++;   
                }
                return;
            }

            _lastPosition = position1;

            if (_sendTimer.Equals(1f))
            {
                _networkController.OnLocalPlayerMove(position, animationState);
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
                animationState = "RUN";
                Rotate();
                idleCountSend = 0;
            }
            else
            {
                animationState = "IDLE";
            }

            var move = transform.forward * verticalMove + transform.right * horizontalMove;
            characterController.Move(playerSpeed * Time.deltaTime * move + gravityMove);
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