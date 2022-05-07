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

    public void onClickSubmit()
    {
        StartCoroutine(RegisterUser(emailInput.text, passwordInput.text));
    }

    public void onClickBack()
    {
        SceneManager.LoadScene("WelcomeMenuScene");

    }

    private static IEnumerator RegisterUser(string email, string password)
    {
        var requestRaw =
            Encoding.UTF8.GetBytes(
                JsonUtility.ToJson(
                    new RegisterRequestModel {email = email, password = password}
                )
            );
        var request = WebRequestBuilder.GetInstance().Request("http://127.0.0.1:8080/api/v1/users", "POST", requestRaw);
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
}