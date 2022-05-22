using System;
using System.Collections;
using System.Text;
using Network;
using UnityEngine;

namespace Forms.CharacterSelection
{
    public class ListController : MonoBehaviour
    {
        public GameObject contentPanel;
        public GameObject characterItemPrefab;
        public string ownerId;
        private void Start()
        {
            ownerId = StaticAccountId.AccountId;
            StartCoroutine(GetAllCharacters(DisplayerCharacteres));
        }

        private void DisplayerCharacteres(Character[] characters)
        {
            if (characters.Length <= 0) return;
            foreach (var character in characters)
            {
                var newCharacter = Instantiate(characterItemPrefab, contentPanel.transform, true);
                var controller = newCharacter.GetComponent<CharacterSelectionItemController>();
                controller.Nickname = character.nickname;
                controller.CharacterId = character._id;
                newCharacter.transform.localScale = Vector3.one;
            }
        }
       private void DestroyAllCharacters()
        {
            var gameObjects = GameObject.FindGameObjectsWithTag ("CharacterSelectionItem");
     
            for(var i = 0 ; i < gameObjects.Length ; i ++)
            {
                Destroy(gameObjects[i]);
            }
        }
       
        private IEnumerator GetAllCharacters(Action<Character[]> characters)
        {
            var request = WebRequestBuilder.GetInstance()
                .Request($"http://127.0.0.1:8080/api/v1/characters/owner/{ownerId}", "GET", null);
            yield return request.SendWebRequest();


            if (request.error != null)
            {
                Debug.Log("Error While Sending: " + request.error);
            }
            else
            {
                if (request.responseCode == 204) yield break;
                var x = JsonUtility.FromJson<Characters>(request.downloadHandler.text);
                characters(x.response);

            }
            yield return new WaitForSeconds(1);
        }

        public void OnCharacterDelete()
        {
            DestroyAllCharacters();
            StartCoroutine(GetAllCharacters(DisplayerCharacteres));
        }
    }
}