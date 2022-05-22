using System;
using System.Collections;
using System.Text;
using Network;
using UnityEngine;

namespace Forms.CharacterSelection
{
    public class ListController : MonoBehaviour
    {
        public GameObject ContentPanel;
        public GameObject CharacterItemPrefab;

        private void Start()
        {
            StartCoroutine(GetAllCharacters(DisplayerCharacteres));
        }

        private void DisplayerCharacteres(Character[] characters)
        {
            foreach (var character in characters)
            {
                var newCharacter = Instantiate(CharacterItemPrefab, ContentPanel.transform, true);
                var controller = newCharacter.GetComponent<CharacterSelectionItemController>();
                controller.Nickname = character.nickname;
                newCharacter.transform.localScale = Vector3.one;
            }
        }

        private IEnumerator GetAllCharacters(Action<Character[]> characters)
        {
            var requestRaw =
                Encoding.UTF8.GetBytes(
                    JsonUtility.ToJson(
                        ""
                    )
                );
            var ownerId = "6285352ea625ff2f821879f4";
            var request = WebRequestBuilder.GetInstance()
                .Request($"http://127.0.0.1:8080/api/v1/characters/owner/{ownerId}", "GET", requestRaw);
            yield return request.SendWebRequest();


            if (request.error != null)
            {
                Debug.Log("Error While Sending: " + request.error);
            }
            else
            {
                Debug.Log("StatusCode: " + request.responseCode + "\nBody: " + request.downloadHandler.text);
                var x = JsonUtility.FromJson<Characters>(request.downloadHandler.text);
                characters(x.response);

            }
            yield return new WaitForSeconds(1);
        }
    }
}