using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuButton : MonoBehaviour
{
    public void GoBackToMenu()
    {
        SceneManager.LoadScene("Home");
    }
}
