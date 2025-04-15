using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameManager gameManager;
    private List<Unit> activeEnemies;

    void Start()
    {
        activeEnemies = new List<Unit>();
        gameManager = FindObjectOfType<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActivateEnemy(Unit unit){
        activeEnemies.Add(unit);
    }
    public List<Unit> GetEnemies(){
        return activeEnemies;
    }
    public void StartPhase(){
        foreach (Unit unit in activeEnemies)
        {
            unit.hasActed = false;
            unit.hasMoved = false;
        }
        StartCoroutine(EnemyPhaseCoroutine());
    }

    private IEnumerator EnemyPhaseCoroutine()
    {
        yield return new WaitForSeconds(3.0f);//wait for phase text
        foreach (Unit unit in activeEnemies){
            unit.EnemyLogic();
            yield return new WaitForSeconds(3.0f);
        }
        gameManager.StartPlayerPhase();
    }

}
