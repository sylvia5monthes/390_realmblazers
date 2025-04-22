using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DemonKingEnemyUnit : Unit
{   
    float[] dark = new float[] {1, 7, 0.9f, 2}; //index 1: physical vs magic. 1 is magic, 0 is phys. then: power, accuracy, and range
    string action0 = "Dark";
    float[] smash = new float[] {0, 7, 0.9f, 1};
    string action1 = "Smash";
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "Demon King";
        actions = new float[][]{
            dark, smash
        };
        actionNames = new string[]{
            action0, action1
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PerformAction(int index)
    {
        base.PerformAction(index);

        Debug.Log("Imp is performing an action!");
    }
    public override void EnemyLogic()
    {
        //finds actable range
        bool actionSelected = false;
        List<Unit> sortedUnits = unitMenuController.GetUnits()
            .OrderBy(unit => gridManager.GetManhattan(currentTilePos, unit.currentTilePos))
            .ToList();
        int sortedIndex = 0;
        Unit closestUnit = this;
        int selectedAction = Random.Range(0, 2);
        int effectiveRange = (int)actions[selectedAction][3] + mov;
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
                        combatManager.HandleCombat(this, closestUnit, actions[selectedAction], actionNames[selectedAction], gridManager.GetManhattan(closestUnit.currentTilePos, currentTilePos));
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
                                combatManager.HandleCombat(this, closestUnit, actions[selectedAction], actionNames[selectedAction], 1);
                                //Debug.Log("attacking distance" + distance);
                                actionSelected = true;
                            } else{
                                gridManager.HighlightEnemyPathTemporarily(currentTilePos, closestInRange);
                                gridManager.MoveEnemy(this, closestInRange);
                                combatManager.HandleCombat(this, closestUnit, actions[selectedAction], actionNames[selectedAction], gridManager.GetManhattan(closestUnit.currentTilePos, currentTilePos));
                                actionSelected = true;
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
                                combatManager.HandleCombat(this, closestUnit, actions[selectedAction], actionNames[selectedAction], gridManager.GetManhattan(closestUnit.currentTilePos, closestInRange));
                                actionSelected=true;
                                //Debug.Log("attacking distance" + gridManager.GetManhattan(closestUnit.currentTilePos, closestInRange));
                            }
                        }
                    }
                }
                sortedIndex+=1;
            }
    }
}
