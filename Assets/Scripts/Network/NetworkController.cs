using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        private const int SERVER_PORT = 5500;
        private const int CLIENT_PORT = 5600;
        
        public TMP_Text networkStatus;
        
        private UdpClient _udpClient;
        private Rect _windowRect;
        private bool _isLoading = true;
        private bool _isConnected;
        private string _userId;

        private Vector3 lastPosition;
        private void Start()
        {
            var x = (Screen.width - 400) / 2;
            var y = (Screen.height - 120) / 2;
            _windowRect = new Rect(x, y, 400, 120);
            _udpClient = new UdpClient(CLIENT_PORT);
            try
            {
                _udpClient.Connect(IP, SERVER_PORT);
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
        

        public void OnLocalPlayerMove(Vector3 position)
        {
            if (!_isConnected) return;
                SendUserMoveEvent(position);
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
            var message = Encoding.ASCII.GetBytes(
                JsonUtility.ToJson(
                    new ConnectEvent
                    {
                        name = "connect",
                        data = new ConnectData
                        {
                            jwtApi = "Bearer " + jwtApi
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
            var message = Encoding.ASCII.GetBytes(
                JsonUtility.ToJson(
                    new DisconnectEvent
                    {
                        name = "disconnect",
                        data = new DisconnectData
                        {
                            authorization = jwtServer,
                            jwtApi = jwtApi,
                            userId = _userId
                        }
                    }
                )
            );
            
            StartCoroutine(SendEvent(message));
            StopCoroutine(nameof(SendEvent));
        }
        
        private void SendUserMoveEvent(Vector3 transformPosition)
        {
            var message = Encoding.ASCII.GetBytes(
                JsonUtility.ToJson(
                    new UserMoveEvent
                    {
                        name = "user-move",
                        data = new UserMoveData
                        {
                            authorization = "Bearer " + PlayerPrefs.GetString("AuthTokenServer"),
                            userId = _userId,
                            position = new Position
                            {
                                x = transformPosition.x,
                                y = transformPosition.y,
                                z = transformPosition.z
                            }
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
            yield return new WaitForSeconds(1f);
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
                        var receiveString = Encoding.ASCII.GetString(receiveBytes);

                        if (receiveString.Contains("connected"))
                            OnConnected(receiveString);
                        if (receiveString.Contains("other-user-move"))
                            OnOtherPlayerMove(receiveString);
                        if (receiveString.Contains("user-disconnected"))
                            OnUserDisconnected(receiveString); 
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }

                yield return null;
            }
        }

        private void OnConnected(string message)
        {
            var connectedEvent = JsonUtility.FromJson<ConnectedEvent>(message);
            PlayerPrefs.SetString("AuthTokenServer", connectedEvent.data.jwtServer);
            Thread.Sleep(2000);
            _userId = connectedEvent.data.user._id;
            _isLoading = false;
            _isConnected = true;
            StopCoroutine(nameof(SendEvent));
        }

        private void OnUserDisconnected(string message)
        {
            Debug.LogError("Tu coś się dzieje xD");
            var userDisconnected = JsonUtility.FromJson<UserDisconnectedEvent>(message);
            Debug.LogError(userDisconnected.data.users[0]._id);
        }
        private void OnOtherPlayerMove(string message)
        {
            var otherUserMoveEvent = JsonUtility.FromJson<OtherUserMoveEvent>(message);
            var position = otherUserMoveEvent.data.position;
            //todo Zmiana pozycji gracza szukanie po ID;
        }
    }
}