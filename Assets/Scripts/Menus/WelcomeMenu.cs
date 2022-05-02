using UnityEngine;
using UnityEngine.SceneManagement;

public class WelcomeMenu : MonoBehaviour
{
   public void OpenLoginScreen()
   {
      SceneManager.LoadScene("LoginScene");
   }

   public void OpenRegisterScreen()
   {
      SceneManager.LoadScene("RegisterScene");
   }
}
