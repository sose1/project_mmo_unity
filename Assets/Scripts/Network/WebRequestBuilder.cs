using UnityEngine.Networking;

namespace Network
{
    public class WebRequestBuilder
    {
        private WebRequestBuilder()
        {
        }

        private static WebRequestBuilder CreateInstance()
        {
            return new WebRequestBuilder();
        }

        private static WebRequestBuilder _instance;

        public static WebRequestBuilder GetInstance()
        {
            if (_instance != null) return _instance;

            _instance = CreateInstance();

            return _instance;
        }

        public UnityWebRequest Request(string url, string methodType, byte[] requestBody)
        {
            var request = new UnityWebRequest(url, methodType)
            {
                uploadHandler = new UploadHandlerRaw(requestBody),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }
    }
}