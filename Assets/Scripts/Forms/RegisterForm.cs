using System;
using System.Collections;
using System.Text;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterForm : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField nicknameInput;

    public void onClickSubmit()
    {
        StartCoroutine(RegisterPlayer(emailInput.text, passwordInput.text, nicknameInput.text));
    }

    public void onClickBack()
    {
        SceneManager.LoadScene("WelcomeMenuScene");

    }

    private static IEnumerator RegisterPlayer(string email, string password, string nickname)
    {
        var requestRaw =
            Encoding.UTF8.GetBytes(
                JsonUtility.ToJson(
                    new RegisterRequestModel {email = email, password = password, nickname = nickname}
                )
            );
        var request = WebRequestBuilder.GetInstance().Request("http://127.0.0.1:8080/api/v1/players", "POST", requestRaw);
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.Log("Error While Sending: " + request.error);
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
        }
    }
}

[Serializable]
public class RegisterRequestModel
{
    public string email;
    public string password;
    public string nickname;
}