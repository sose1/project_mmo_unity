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
    public TMP_Text errorMessage;

    public void onClickLogin()
    {
        StartCoroutine(Login(emailInput.text, passwordInput.text, message => errorMessage.SetText(message)));
    }

    public void onClickBack()
    {
        SceneManager.LoadScene("WelcomeMenuScene");
    }
    
    
    private static IEnumerator Login(string email, string password, Action<string> callback)
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

        if (request.error != null)
        {
            Debug.Log("Error While Sending: " + request.error);
            if (request.error.Contains("409"))
                callback("Błędne dane logowania!");
            else if (request.error.Contains("403"))
                callback("Gracz jest już zalogowany!");
            else 
                callback("Nieznany błąd!");
        }
        else
        {
            Debug.Log("StatusCode: " + request.responseCode + "\nBody: " + request.downloadHandler.text);
            if (request.responseCode != 200) yield break;
            
            PlayerPrefs.SetString("AuthTokenAPI", request.downloadHandler.text);
            SceneManager.LoadScene("GameScene");
        }

        yield return new WaitForSeconds(3);
        callback("");
    }
}

[Serializable]
public class LoginRequestModel
{
    public string email;
    public string password;
}