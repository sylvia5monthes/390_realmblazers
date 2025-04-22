using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    
    public void GoToLevelOne()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void GoToLevelTwo()
    {
        SceneManager.LoadScene("Stage2");
    }

    public void GoToLevelThree()
    {
        SceneManager.LoadScene("Stage3");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

}
