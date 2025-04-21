using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    public void BeginGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

}
