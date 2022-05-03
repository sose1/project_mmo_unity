using Network;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters
{
    public class LocalPlayerController : MonoBehaviour
    {
        [FormerlySerializedAs("CharacterController")]
        public CharacterController characterController;

        public float speed = 5f;

        private GameObject _networkControllerGameObject;
        private NetworkController _networkController;
        private Vector3 _lastPosition;

        private void Start()
        {
            _networkControllerGameObject = GameObject.Find("NetworkController");
            _networkController = _networkControllerGameObject.GetComponent<NetworkController>();
        }

        private void Update()
        {
            Move();

            if (_lastPosition == transform.position) return;

            var position = transform.position;
            _networkController.OnLocalPlayerMove(position);
            _lastPosition = position;
        }

        private void Move()
        {
            var horizontalMove = Input.GetAxis("Horizontal");
            var verticalMove = Input.GetAxis("Vertical");

            var move = transform.forward * verticalMove + transform.right * horizontalMove;
            characterController.Move(speed * Time.deltaTime * move);
        }
    }
}