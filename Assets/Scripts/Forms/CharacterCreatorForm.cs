using System;
using System.Collections;
using System.Text;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Forms
{
    public class CharacterCreatorForm : MonoBehaviour
    {
        public TMP_InputField nickname;

        public void OnCreateClick()
        {
            StartCoroutine(CreateCharacter(nickname.text));
        }

        public void OnBackClick()
        {
            SceneManager.LoadScene("CharacterSelectionScene");
        }
        
        private IEnumerator CreateCharacter(string nickname)
        {
            var requestRaw =
                Encoding.UTF8.GetBytes(
                    JsonUtility.ToJson(
                        new CharacterModel {nickname = nickname}
                    )
                );
            var request = WebRequestBuilder.GetInstance().Request($"http://127.0.0.1:8080/api/v1/characters/owner/{StaticAccountId.AccountId}", "POST", requestRaw);
            yield return request.SendWebRequest();

            if (request.error != null)
            {
                Debug.Log("Error While Sending: " + request.error);
            }
            else
            {
                Debug.Log("Received: " + request.downloadHandler.text);
                SceneManager.LoadScene("CharacterSelectionScene");
            }
        }
    }
    
    [Serializable]
    public class CharacterModel
    {
        public string nickname;
    }
}