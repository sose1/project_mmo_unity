using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Network.Models.RequestEvent;
using Network.Models.ResponseEvent;
using UnityEngine;
using Data = Network.Models.RequestEvent.Data;

namespace Network
{
    public class NetworkController : MonoBehaviour
    {
        private UdpClient _udpClient;
        private Rect _windowRect;
        private bool _isLoading = true;
        private bool _isConnected = false;
        private void Start()
        {
            var x = (Screen.width - 400) / 2;
            var y = (Screen.height - 120) / 2;
            _windowRect = new Rect(x, y, 400, 120);
            _udpClient = new UdpClient(5600);
            try
            {
                _udpClient.Connect("127.0.0.1", 5500);

            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            SendConnectEvent();
        }

        private void OnDestroy()
        {
            PlayerPrefs.DeleteAll();
            StopAllCoroutines();
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
                    new ConnectEvent()
                    {
                        name = "connect",
                        data = new Data()
                        {
                            jwtApi = "Bearer " + jwtApi
                        }
                    }
                )
            );
            return message;
        }

        private void SendConnectEvent()
        {
            StartCoroutine(SendEvent(GetConnectEventMessage(), message =>
            {
                var connectedEvent = JsonUtility.FromJson<ConnectedEvent>(message);
                PlayerPrefs.SetString("AuthTokenServer", connectedEvent.data.jwtServer);
                Thread.Sleep(2000);
                _isLoading = false;
                _isConnected = true;
                StopCoroutine(nameof(SendEvent));
            }));
        }

        private IEnumerator SendEvent(byte[] message, Action<string> onMessageReceived)
        {
            var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            yield return _udpClient.Send(message, message.Length);

            var receiveBytes = _udpClient.Receive(ref remoteEndPoint);
            var receiveString = Encoding.ASCII.GetString(receiveBytes);

            onMessageReceived(receiveString);
            yield return null;
        }
    }
}