using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Unit : MonoBehaviour
{
    public string unitDisplayName = "";
    public bool isEnemy = false;
    public bool isMinion = false;
    public bool isBoss = false;

    // stats fields
    public float health;
    public float atk;
    public float def;
    public float matk;
    public float mdef;
    public float prec;
    public float eva;
    public int mov = 1;


    // current state fields
    public Vector3Int currentTilePos;
    public bool hasMoved = false;
    public bool hasActed = false;
    public float currentHealth;
    public float[][] actions;
    public string[] actionNames;
    protected GridManager gridManager;
    protected UnitMenuController unitMenuController;
    protected CombatManager combatManager;
    private int buffTurns = 0;
    public bool defenseBuffed = false;
    public bool defenseDebuffed = false;

    protected virtual void Start()
    {
        currentHealth = health;
        gridManager = FindObjectOfType<GridManager>();
        unitMenuController = FindObjectOfType<UnitMenuController>();
        combatManager = FindObjectOfType<CombatManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo(Vector3Int newTilePos)
    {
        currentTilePos = newTilePos;
        transform.position = FindObjectOfType<Tilemap>().GetCellCenterWorld(newTilePos);
    }

    void OnMouseDown()
    {   
        if (GameManager.Instance.currentPhase != GameManager.GamePhase.PlayerPhase ||
            GameManager.Instance.isPhaseChanging || IsPointerOverUI())
        {
            Debug.Log("Not in player phase or UI is clicked");
            return;
        }

        if (gridManager.IsInTargetingMode() && isEnemy)
        {
            Debug.Log("In targeting mode");
            return; // Let GridManager handle it from Update()
        }

        Debug.Log($"Unit {gameObject.name} was clicked!");
        UnitMenuController actionMenu = FindObjectOfType<UnitMenuController>();
        if (actionMenu != null)
        {
            actionMenu.ShowUnitActionMenu(this);
        }
    }

    private bool IsPointerOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    // can be overriden by specific unit classes
    public virtual void PerformAction(int index) {

        // add generic action logic here
    }//redefining in below UseAbility code so that i don't have to manually update every script- we might be able to repurpose this (ie, items?)
    //the point is i want the child/specific unit class to be able to call it with the index but pass in the actual float itself here
    //we can also restructure this later if this is a mess
    public void UseAbility(float[] ability, Unit unit){

    }
    public void UseAbility(float[] ability){//overloading? we might not need this but we'll probably have targeted and non-targeted abilities

    }
    public virtual void EnemyLogic(){//can be overridden with better AI later. default is attack closest unit using 1st ability
        //finds actable range
        bool actionSelected = false;
        int effectiveRange = (int)actions[0][3] + mov;
        List<Unit> sortedUnits = unitMenuController.GetUnits()
            .OrderBy(unit => gridManager.GetManhattan(currentTilePos, unit.currentTilePos))
            .ToList();
        int sortedIndex = 0;
        Unit closestUnit = this;
        while (actionSelected == false)
            {
                if(sortedIndex>sortedUnits.Count){
                    //donothing
                    return;
                }
                closestUnit = sortedUnits[sortedIndex];
                int distance = gridManager.GetManhattan(currentTilePos,closestUnit.currentTilePos);
                if (distance > effectiveRange){
                    actionSelected = true;//stop searching
                    Vector3Int closest = currentTilePos;
                    distance = 999;//moving distance, separate from manhattan distance to enem ysorry for same variable name
                    foreach(Vector3Int gridPos in gridManager.GetMovableTiles(currentTilePos, mov, true)){
                        if (gridManager.GetManhattan(gridPos, closestUnit.currentTilePos) < distance){
                            closest = gridPos;
                            distance = gridManager.GetManhattan(gridPos, closestUnit.currentTilePos);
                        }
                        Debug.Log("current closest: " + closest);
                    }
                    gridManager.HighlightEnemyPathTemporarily(currentTilePos, closest);
                    gridManager.MoveEnemy(this, closest);
                    Debug.Log("moving distance" + distance);
                } else{
                    //if there are units in range but attackable tiles are occupied, search for a different target.
                    int range = effectiveRange-mov;
                    Vector3Int closestInRange = currentTilePos;
                    int manhattanToUnit = gridManager.GetManhattan(currentTilePos, closestUnit.currentTilePos);
                    if (manhattanToUnit == range){
                        gridManager.HighlightEnemyPathTemporarily(currentTilePos, closestInRange);//stand in place and attack
                        combatManager.HandleCombat(this, closestUnit, actions[0], actionNames[0], 1);
                        //Debug.Log("attacking distance" + distance);
                        actionSelected = true;
                    } else{
                        distance = mov+1;//moving distance, not same as manhattan distance to enemy. sorry for same variable name
                        if(manhattanToUnit < range){//ranged attacker in melee range
                            foreach(Vector3Int gridPos in gridManager.GetMovableTiles(currentTilePos, mov, true)){
                                if (gridManager.GetManhattan(gridPos, currentTilePos) < distance && gridManager.GetManhattan(closestUnit.currentTilePos, gridPos) == range){
                                    closestInRange = gridPos;
                                    distance = gridManager.GetManhattan(gridPos, currentTilePos);
                                }
                            }
                            if (closestInRange.Equals(currentTilePos)){
                                gridManager.HighlightEnemyPathTemporarily(currentTilePos, closestInRange);//stand in place and attack
                                combatManager.HandleCombat(this, closestUnit, actions[0], actionNames[0], 1);
                                //Debug.Log("attacking distance" + distance);
                                actionSelected = true;
                            } else{
                                gridManager.HighlightEnemyPathTemporarily(currentTilePos, closestInRange);
                                gridManager.MoveEnemy(this, closestInRange);
                                combatManager.HandleCombat(this, closestUnit, actions[0], actionNames[0], 1);
                                //Debug.Log("attacking distance" + distance);
                            }
                        } else{//out of range
                            foreach(Vector3Int gridPos in gridManager.GetMovableTiles(currentTilePos, mov, true)){
                                if (gridManager.GetManhattan(gridPos, currentTilePos) < distance && gridManager.GetManhattan(closestUnit.currentTilePos, gridPos) <= range){
                                    closestInRange = gridPos;
                                    distance = gridManager.GetManhattan(gridPos, currentTilePos);
                                }
                            }
                            if (closestInRange.Equals(currentTilePos)){
                                //do nothing, actionselected should still be false
                            } else{
                                gridManager.HighlightEnemyPathTemporarily(currentTilePos, closestInRange);
                                gridManager.MoveEnemy(this, closestInRange);
                                combatManager.HandleCombat(this, closestUnit, actions[0], actionNames[0], gridManager.GetManhattan(closestUnit.currentTilePos, closestInRange));
                                actionSelected=true;
                                //Debug.Log("attacking distance" + gridManager.GetManhattan(closestUnit.currentTilePos, closestInRange));
                            }
                        }
                    }
                }
                sortedIndex+=1;
            }
    }

    public void HandleDeathCleanup()
    {
        GridManager gridManager = FindObjectOfType<GridManager>();
        UnitMenuController menu = FindObjectOfType<UnitMenuController>();
        CombatTextController combatUI = FindObjectOfType<CombatTextController>();

        gridManager?.RemoveUnit(currentTilePos);

        if (!isEnemy)
        {
            menu?.GetUnits().Remove(this);
            menu?.HideUnitActionMenu();
        }

        combatUI?.ShowDeathText(unitDisplayName);
        Destroy(gameObject);
    }
    public virtual void CheckDeath()
    {
        if (currentHealth <= 0)
        {
            Debug.Log($"{unitDisplayName} has been defeated.");
            GameManager.Instance.OnUnitDeath(this);
        }
    }
    public int GetLevel(){
        return unitMenuController.level-1;//want to scale based on level - 1, not level itself
    }
    public void SetDefenseBuff(bool isBuff, int turns){
        buffTurns = turns;
        defenseBuffed = false;
        defenseDebuffed = false;
        if (isBuff){
            defenseBuffed = true;
        } else{
            defenseDebuffed = true;
        }
    }
    public void DecrementBuff(){
        if (buffTurns >0){
            buffTurns-=1;
        }
        if (buffTurns == 0){
            defenseBuffed = false;
            defenseDebuffed = false;
        }
    }

}
