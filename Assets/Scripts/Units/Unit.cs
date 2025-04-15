using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Unit : MonoBehaviour
{
    public string unitDisplayName = "";
    public bool isEnemy = false;

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
    private GridManager gridManager;
    private UnitMenuController unitMenuController;
    private CombatManager combatManager;

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
            GameManager.Instance.isPhaseChanging)
        {
            Debug.Log("Not your turn!");
            return;
        }
        Debug.Log($"Unit {gameObject.name} was clicked!");
        UnitMenuController actionMenu = FindObjectOfType<UnitMenuController>();
        if (actionMenu != null)
        {
            actionMenu.ShowUnitActionMenu(this);
        }
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
        int effectiveRange = (int)actions[0][3] + mov;
        int distance = 999;
        Unit closestUnit = this;
        foreach (Unit unit in unitMenuController.GetUnits())
            {
                if (gridManager.GetManhattan(currentTilePos,unit.currentTilePos) < distance){
                    closestUnit = unit;
                    distance = gridManager.GetManhattan(currentTilePos,unit.currentTilePos);
                }
            }
        if (distance > effectiveRange){//move to tile closest to this unit
            Vector3Int closest = currentTilePos;
            distance = 999;
            foreach(Vector3Int gridPos in gridManager.GetMovableTiles(currentTilePos, mov, true)){
                if (gridManager.GetManhattan(gridPos, closestUnit.currentTilePos) < distance){
                    closest = gridPos;
                    distance = gridManager.GetManhattan(gridPos, closestUnit.currentTilePos);
                }
            }
            gridManager.HighlightEnemyPathTemporarily(currentTilePos, closest);
            gridManager.MoveEnemy(this, closest);
            Debug.Log(distance);
        }
        else{//move to closest tile in range of enemy and attack
            int range = effectiveRange-mov;
            Vector3Int closestInRange = currentTilePos;
            distance = mov+1;
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) == range)
                    {
                        Vector3Int newTile = new Vector3Int(closestUnit.currentTilePos.x + dx, closestUnit.currentTilePos.y + dy, 0);
                        if (gridManager.GetManhattan(currentTilePos, newTile) < distance && gridManager.IsInBounds(newTile) && !gridManager.TileAt(newTile).IsOccupied){
                            closestInRange = newTile;
                            distance = gridManager.GetManhattan(currentTilePos, newTile);
                        }
                    }
                }
            }
            if (closestInRange.Equals(currentTilePos)){
                //do nothing
            } else{
                gridManager.HighlightEnemyPathTemporarily(currentTilePos, closestInRange);
                gridManager.MoveEnemy(this, closestInRange);
                combatManager.HandleCombat(this, closestUnit, actions[0], range);
                Debug.Log(distance);
            }
        }
    }

    public virtual void CheckDeath()
    {
        if (currentHealth <= 0)
        {
            Debug.Log($"{unitDisplayName} has been defeated.");

            var grid = FindObjectOfType<GridManager>();
            var menu = FindObjectOfType<UnitMenuController>();
            var combatUI = FindObjectOfType<CombatTextController>();

            grid?.RemoveUnit(currentTilePos);
            if (!isEnemy){
                menu?.GetUnits().Remove(this);
                menu?.HideUnitActionMenu();
            }
    
        

            combatUI?.ShowDeathText(unitDisplayName); // show "___ has fallen!"
            Destroy(gameObject);
        }
    }

}
