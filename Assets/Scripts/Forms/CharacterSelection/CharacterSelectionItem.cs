using System.Collections;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Forms.CharacterSelection
{
    public class CharacterSelectionItem: MonoBehaviour
    {
        public TMP_Text NicknameText;
        public string Nickname;
        public string CharacterId;
        private void Start()
        {
            NicknameText.text = Nickname;
        }

        public void OnDeleteClick()
        {
            StartCoroutine(DeleteCharacter());
        }

        public void OnPlayClick()
        {
            SceneManager.LoadScene("GameScene");
        }

        private IEnumerator DeleteCharacter()
        {
            var request = WebRequestBuilder.GetInstance()
                .Request($"http://127.0.0.1:8080/api/v1/characters/{CharacterId}/owner/{StaticAccountId.AccountId}", "DELETE", null);
            yield return request.SendWebRequest();
            
            
            if (request.error != null)
            {
                Debug.Log("Error While Sending: " + request.error);
            }
            else
            {
                Debug.Log("StatusCode: " + request.responseCode + "\nBody: " + request.downloadHandler.text);
                GetComponentInParent<CharacterSelectionAdapter>().OnCharacterDelete();
            }

            yield return 0;
        }
    }
}