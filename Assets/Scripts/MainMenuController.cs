using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public void GoToThegame(int index)
    {
        SceneManager.LoadScene(index);
    }


    public void Exit()
    {
        Application.Quit();
    }
}
