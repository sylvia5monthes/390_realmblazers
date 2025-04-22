using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// handles the pause menu functionality
// allows the player to pause the game, resume, restart, or quit
public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; 
            pauseMenu.SetActive(true); 
        }
        else
        {
            Time.timeScale = 1f; 
            pauseMenu.SetActive(false); 
        }
    }

    public void ResumeGame()
    {
        TogglePause(); 
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("StartMenu");
        Time.timeScale = 1f;
    }

}
