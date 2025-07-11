using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    private GameManager gameManager;
    private List<Unit> activeMinions = new List<Unit>();
    private List<Unit> activeBosses = new List<Unit>();
    private bool enteredBossPhase = false;
    private GridManager gridManager;
    public GameObject magmaDamageDisplay;
    private DialogueManager dialogueManager;
    public Sequence endDialogue;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gridManager = FindObjectOfType<GridManager>();

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
    
    // get all active enemies (not dead)
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

        // Check if all minions are defeated and trigger boss phase
        if (activeMinions.Count == 0 && !enteredBossPhase && GameManager.Instance.currentPhase != GameManager.GamePhase.BossPhase)
        {
            enteredBossPhase = true;
            Debug.Log("[OnEnemyDeath] All minions defeated. Triggering Boss Phase.");
            StartCoroutine(TriggerBossPhaseThenPlayer());
        }
        // check if boss has been defeated
        else if (activeBosses.Count == 0 && enteredBossPhase)
        {
            Debug.Log("[OnEnemyDeath] All bosses defeated. Loading next scene.");
            
            dialogueManager = FindObjectOfType<DialogueManager>();

            dialogueManager.StartEndDialogue(endDialogue);
        }
    }

    // start enemy phase
    public void StartPhase()
    {
        Tilemap tilemap = FindObjectOfType<Tilemap>();
        List<Unit> validEnemies = new List<Unit>();
        // iterate through all enemies to check if they are alive and if so initialize attributes
        foreach (Unit unit in GetEnemies())
        {
            if (unit.currentHealth > 0)
            {
                unit.hasMoved = false;
                unit.hasActed = false;
                validEnemies.Add(unit);
                //if (gridManager.TileAt(unit.currentTilePos).isMagma){
                //    unit.SetDefenseBuff(false, 2);
                //}
                unit.DecrementBuff();
            }
        }

        StartCoroutine(EnemyPhaseCoroutine(validEnemies));
    }

    private IEnumerator EnemyPhaseCoroutine(List<Unit> enemies)
    {
        yield return new WaitForSeconds(4.0f); // wait for phase text
        // iterate through all enemies to check if they are alive and if so perform actions through enemy logic
        foreach (Unit unit in enemies)
        {
            if (unit.currentHealth > 0)
            {
                unit.EnemyLogic();
                if (gridManager.TileAt(unit.currentTilePos).isMagma && unit.currentHealth > 0)
                {
                        Debug.Log("Magma damage");
                        Tilemap tilemap = FindObjectOfType<Tilemap>();
                        unit.currentHealth -= 3;
                        Vector3 displayPos = tilemap.GetCellCenterWorld(gridManager.GridPositionToWorldPosition(unit.currentTilePos));
                        Instantiate(magmaDamageDisplay, displayPos, Quaternion.identity);
                        unit.CheckDelayDeath();
                    }
                yield return new WaitForSeconds(3.0f);
            }
        }
        yield return new WaitForSeconds(2.0f);
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
