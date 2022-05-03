using System;
using System.Collections;
using System.Text;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginForm : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    public void onClickLogin()
    {
        StartCoroutine(Login(emailInput.text, passwordInput.text));
    }

    public void onClickBack()
    {
        SceneManager.LoadScene("WelcomeMenuScene");
    }

    private static IEnumerator Login(string email, string password)
    {
        var requestRaw =
            Encoding.UTF8.GetBytes(
                JsonUtility.ToJson(
                    new RegisterRequestModel {email = email, password = password}
                )
            );
        var request = WebRequestBuilder.GetInstance()
            .Request("http://127.0.0.1:8080/api/v1/users/login", "POST", requestRaw);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
        }
        else
        {
            Debug.Log("StatusCode: " + request.responseCode + "\nBody: " + request.downloadHandler.text);
            if (request.responseCode != 200) yield break;
            
            PlayerPrefs.SetString("AuthTokenAPI", request.downloadHandler.text);
            SceneManager.LoadScene("GameScene");
        }
    }
}

[Serializable]
public class LoginRequestModel
{
    public string email;
    public string password;
}