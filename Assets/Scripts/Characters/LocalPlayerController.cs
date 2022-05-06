using System;
using Network;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters
{
    public class LocalPlayerController : MonoBehaviour
    {
        [FormerlySerializedAs("CharacterController")]
        public CharacterController characterController;

        public float speed = 200f;
        public string id;
        private GameObject _networkControllerGameObject;
        private NetworkController _networkController;
        private Vector3 _lastPosition;
        private float _sendTImer = 0f;
        private void Start()
        {
            Debug.Log("TWorze gracza o ID: " + id);
            _networkControllerGameObject = GameObject.Find("NetworkController");
            _networkController = _networkControllerGameObject.GetComponent<NetworkController>();
        }

        private void FixedUpdate()
        {
            Move();

            if (_lastPosition == transform.position) return;
            
            var position = transform.position;
            _lastPosition = position;
            if (_sendTImer.Equals(1f))
            {
                _networkController.OnLocalPlayerMove(position);
                _sendTImer = 0f;
            }
            else
            {
                _sendTImer++;
            }
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