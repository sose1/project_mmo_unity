using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cinemachine;
using Network.Models.Other;
using Network.Models.RequestEvent;
using Network.Models.ResponseEvent;
using TMPro;
using UnityEngine;

namespace Network
{
    public class NetworkController : MonoBehaviour
    {
        private const string IP = "127.0.0.1";
        private const int ServerPort = 5500;
        private const int ClientPort = 0;

        public TMP_Text networkStatus;
        public GameObject localPlayerPrefab;
        public GameObject remotePlayerPrefab;

        private UdpClient _udpClient;
        private Rect _windowRect;
        private bool _isLoading = true;
        private bool _isConnected;
        private string _playerId;

        private void Start()
        {
            var x = (Screen.width - 400) / 2;
            var y = (Screen.height - 120) / 2;
            _windowRect = new Rect(x, y, 400, 120);
            _udpClient = new UdpClient(ClientPort);
            Debug.Log("UDP port : " + ((IPEndPoint) _udpClient.Client.LocalEndPoint).Port.ToString());

            try
            {
                _udpClient.Connect(IP, ServerPort);
                _udpClient.Client.ReceiveTimeout = 1000;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            StartCoroutine(PingUpdate());
            StartCoroutine(OnReceive());
            StartCoroutine(SendEvent(GetConnectEventMessage()));
        }

        public void OnLocalPlayerMove(Position position, string animationState)
        {
            if (!_isConnected) return;
            SendPlayerMoveEvent(position, animationState);
        }

        private void OnApplicationQuit()
        {
            SendDisconnectEvent();
        }

        private void OnDestroy()
        {
            PlayerPrefs.DeleteAll();
            StopAllCoroutines();
            _udpClient.Close();
        }

        private void OnGUI()
        {
            if (_isLoading)
            {
                _windowRect = GUI.Window(0, _windowRect, null, "Loading...");
            }
        }

        private static byte[] GetConnectEventMessage()
        {
            var jwtApi = PlayerPrefs.GetString("AuthTokenAPI");
            var message = Encoding.UTF8.GetBytes(
                JsonUtility.ToJson(
                    new ConnectEvent
                    {
                        name = "connect",
                        data = new ConnectData
                        {
                            jwtApi = "Bearer " + jwtApi,
                            characterId = StaticCharacterId.CharacterId
                        }
                    }
                )
            );
            return message;
        }

        private void SendDisconnectEvent()
        {
            var jwtApi = "Bearer " + PlayerPrefs.GetString("AuthTokenAPI");
            var jwtServer = "Bearer " + PlayerPrefs.GetString("AuthTokenServer");
            var message = Encoding.UTF8.GetBytes(
                JsonUtility.ToJson(
                    new DisconnectEvent
                    {
                        name = "disconnect",
                        data = new DisconnectData
                        {
                            authorization = jwtServer,
                            jwtApi = jwtApi,
                            playerId = _playerId
                        }
                    }
                )
            );

            StartCoroutine(SendEvent(message));
            StopCoroutine(nameof(SendEvent));
        }

        private void SendPlayerMoveEvent(Position transformPosition, string animationState)
        {
            networkStatus.text = $"Rotation: {transformPosition.rotation}";

            var message = Encoding.UTF8.GetBytes(
                JsonUtility.ToJson(
                    new PlayerMoveEvent
                    {
                        name = "player-move",
                        data = new PlayerMoveData
                        {
                            authorization = "Bearer " + PlayerPrefs.GetString("AuthTokenServer"),
                            playerId = _playerId,
                            position = transformPosition,
                            animationState = animationState
                        }
                    }
                )
            );

            StartCoroutine(SendEvent(message));
            StopCoroutine(nameof(SendEvent));
        }

        private IEnumerator PingUpdate()
        {
            RestartLoop:

            var ping = new Ping(IP);
            yield return new WaitForSeconds(3f);
            while (!ping.isDone) yield return null;

            networkStatus.text = $"SERVER: {ping.ip}, PING: {ping.time} ms";
            goto RestartLoop;
        }

        private IEnumerator SendEvent(byte[] message)
        {
            yield return _udpClient.Send(message, message.Length);
        }

        private IEnumerator OnReceive()
        {
            while (true)
            {
                try
                {
                    if (_udpClient.Available > 0)
                    {
                        var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                        var receiveBytes = _udpClient.Receive(ref remoteEndPoint);
                        var receiveString = Encoding.UTF8.GetString(receiveBytes);
                        if (receiveString.Contains("player-disconnected"))
                            OnPlayerDisconnected(receiveString);
                        else if (receiveString.Contains("player-connected"))
                            OnPlayerConnected(receiveString);
                        else if (receiveString.Contains("connected"))
                            OnConnected(receiveString);
                        else if (receiveString.Contains("other-player-move"))
                            OnOtherPlayerMove(receiveString);

                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                yield return null;
            }
        }


        private void OnConnected(string message)
        {
            var connectedEvent = JsonUtility.FromJson<ConnectedEvent>(message);
            PlayerPrefs.SetString("AuthTokenServer", connectedEvent.data.jwtServer);
            Thread.Sleep(2000);
            _playerId = connectedEvent.data.player._id;
            _isLoading = false;
            _isConnected = true;

            //Generate local player
            var lastPosition = connectedEvent.data.player.position;
            CreateLocalPlayer(lastPosition);

            //Generate RemotePlayer
            var remotePlayers = connectedEvent.data.otherPlayers;
            if (remotePlayers != null && remotePlayers.Count > 0)
            {
                foreach (var player in remotePlayers)
                {
                    if (player._id != _playerId)
                    {
                        CreateRemotePlayer(player.position, player);
                    }
                }
            }
        }


        private void OnPlayerConnected(string receiveString)
        {
            var playerConnectedEvent = JsonUtility.FromJson<PlayerConnectedEvent>(receiveString);
            var lastPosition = playerConnectedEvent.data.player.position;

            if (playerConnectedEvent.data.player._id != _playerId)
            {
                CreateRemotePlayer(lastPosition, playerConnectedEvent.data.player);
            }
        }

        private void OnPlayerDisconnected(string message)
        {
            var playerDisconnectedEvent = JsonUtility.FromJson<PlayerDisconnectedEvent>(message);
            var remotePlayerId = playerDisconnectedEvent.data.player._id;
            Destroy(GameObject.Find(remotePlayerId));
        }

        private void OnOtherPlayerMove(string message)
        {
            var otherPlayerMoveEvent = JsonUtility.FromJson<OtherPlayerMoveEvent>(message);
            var position = otherPlayerMoveEvent.data.position;
            Debug.Log($"OTHER PLAYER STATE {otherPlayerMoveEvent.data.animationState}");
            GameObject.Find(otherPlayerMoveEvent.data.playerId).GetComponent<RemotePlayerController>().Move(position);
        }

        private void CreateLocalPlayer(Position position)
        {
            Instantiate(
                localPlayerPrefab,
                position != null
                    ? new Vector3(position.x, position.y + 2, position.z)
                    : new Vector3(0, 5, 0),
                Quaternion.Euler(0, position.rotation, 0)
            );
        }

        private void CreateRemotePlayer(Position position, Player remotePlayer)
        {
            Instantiate(
                remotePlayerPrefab,
                new Vector3(position.x, position.y, position.z),
                Quaternion.identity
            ).name = remotePlayer._id;
            GameObject.Find(remotePlayer._id).GetComponentInChildren<TextMeshPro>().text = remotePlayer.nickname;

            var cameraRotation = FindObjectOfType<CinemachineVirtualCamera>().transform.position;
            var textRotation = Quaternion.Euler(cameraRotation.x, 0, cameraRotation.z);
            GameObject.Find(remotePlayer._id)
                .GetComponentInChildren<TextMeshPro>()
                .transform.rotation = textRotation;
        }
    }
}