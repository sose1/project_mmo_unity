using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionForm : MonoBehaviour
{
    public void OnCreateCharacterClick()
    {
        SceneManager.LoadScene("CharacterCreatorScene");
    }
}
