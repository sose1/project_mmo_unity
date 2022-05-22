using System.Collections;
using Network;
using TMPro;
using UnityEngine;

namespace Forms.CharacterSelection
{
    public class CharacterSelectionItemController: MonoBehaviour
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
            Debug.Log($"Usuwam {Nickname}, OwnerID: {StaticAccountId.AccountId}");
            StartCoroutine(DeleteCharacter());
        }

        public void OnPlayClick()
        {
            Debug.Log($"Gram {Nickname}");
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
                GetComponentInParent<ListController>().OnCharacterDelete();
            }

            yield return 0;
        }
    }
}