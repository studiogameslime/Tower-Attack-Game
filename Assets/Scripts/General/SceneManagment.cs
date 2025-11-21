using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagment : MonoBehaviour
{
    public static SceneManagment Instance;

    private void Awake()
    {
        // סינגלטון פשוט
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }
    public void GoToBattle()
    {
        SceneManager.LoadScene("Chapter 1-1");
    }

}
