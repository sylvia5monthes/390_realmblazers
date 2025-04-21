using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameManager gameManager;
    private List<Unit> activeMinions = new List<Unit>();
    private List<Unit> activeBosses = new List<Unit>();
    private bool enteredBossPhase = false;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActivateEnemy(Unit unit)
    {
        if (unit.isMinion)
        {
            activeMinions.Add(unit);
        }
        else if (unit.isBoss)
        {
            activeBosses.Add(unit);
        }
        Debug.Log("EnemyController: Added " + unit.unitDisplayName + " to enemies.");
    }
    public List<Unit> GetEnemies()
    {
        List<Unit> all = new List<Unit>();
        all.AddRange(activeMinions);
        all.AddRange(activeBosses);
        return all;
    }
    public void OnEnemyDeath(Unit unit)
    {
        Debug.Log($"[OnEnemyDeath] {unit.unitDisplayName} has died.");

        if (unit.isMinion && activeMinions.Contains(unit))
        {
            activeMinions.Remove(unit);
        }

        if (unit.isBoss && activeBosses.Contains(unit))
        {
            activeBosses.Remove(unit);
        }

        unit.HandleDeathCleanup();

        Debug.Log($"[OnEnemyDeath] Minions left: {activeMinions.Count}, Bosses left: {activeBosses.Count}");

        if (activeMinions.Count == 0 && !enteredBossPhase && GameManager.Instance.currentPhase != GameManager.GamePhase.BossPhase)
        {
            enteredBossPhase = true;
            Debug.Log("[OnEnemyDeath] All minions defeated. Triggering Boss Phase.");
            StartCoroutine(TriggerBossPhaseThenPlayer());
        }
        else if (activeBosses.Count == 0 && enteredBossPhase)
        {
            Debug.Log("[OnEnemyDeath] All bosses defeated. Loading next scene.");
            gameManager.LoadNext();
        }
    }
    public void StartPhase()
    {
        List<Unit> validEnemies = new List<Unit>();
        foreach (Unit unit in GetEnemies())
        {
            if (unit.currentHealth > 0)
            {
                unit.hasMoved = false;
                unit.hasActed = false;
                validEnemies.Add(unit);
            }
        }

        StartCoroutine(EnemyPhaseCoroutine(validEnemies));
    }

    private IEnumerator EnemyPhaseCoroutine(List<Unit> enemies)
    {
        yield return new WaitForSeconds(3.0f); // wait for phase text
        foreach (Unit unit in enemies)
        {
            if (unit.currentHealth > 0)
            {
                unit.EnemyLogic();
                yield return new WaitForSeconds(3.0f);
            }
        }
        gameManager.StartPlayerPhase();
    }

    private IEnumerator TriggerBossPhaseThenPlayer()
    {
        Debug.Log("[TriggerBossPhaseThenPlayer] Starting boss phase.");

        GameManager.Instance.currentPhase = GameManager.GamePhase.BossPhase;
        GameManager.Instance.isPhaseChanging = true;

        FindObjectOfType<PhaseTextController>()?.ShowPhase("Boss Phase");
        yield return new WaitForSeconds(3f);

        Debug.Log("[TriggerBossPhaseThenPlayer] Spawning boss...");
        FindObjectOfType<GridManager>()?.SpawnBoss();

        Debug.Log("[TriggerBossPhaseThenPlayer] Switching back to Player Phase.");
        GameManager.Instance.StartPlayerPhase();
    }

}
