using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Loading");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CreditScene()
    {
        SceneManager.LoadScene("CreditScene");
    }

    public void MainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
