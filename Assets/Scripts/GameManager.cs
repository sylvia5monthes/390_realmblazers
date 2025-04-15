using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public enum GamePhase {
        Placement, 
        PlayerPhase,
        PlayerCounter,
        EnemyPhase,
        EnemyCounter,
        GameOver

    }

    public GamePhase currentPhase;
    public bool isPhaseChanging = false;

    // Start is called before the first frame update
    void Start()
    {
        currentPhase = GamePhase.Placement;
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPlayerPhase()
    {
        currentPhase = GamePhase.PlayerPhase;
        isPhaseChanging = true;
        FindObjectOfType<PhaseTextController>()?.ShowPhase("Player Phase");
        FindObjectOfType<UnitMenuController>()?.PhaseStart();
        Debug.Log("Player Phase Started");
    }

    public void StartEnemyPhase()
    {
        currentPhase = GamePhase.EnemyPhase;
        isPhaseChanging = true;
        FindObjectOfType<PhaseTextController>()?.ShowPhase("Enemy Phase");
        FindObjectOfType<EnemyController>()?.StartPhase();
        Debug.Log("Enemy Phase Started");
    }
    public void LoadEnd(){
        SceneManager.LoadScene("EndDemo");
    }

}
