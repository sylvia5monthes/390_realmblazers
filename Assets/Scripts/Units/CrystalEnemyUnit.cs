using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class CrystalEnemyUnit : Unit
{   
    float[] bolt = new float[] {1, 7, 0.9f, 3}; //index 1: physical vs magic. 1 is magic, 0 is phys. then: power, accuracy, and range
    string action0 = "Bolt";
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "Crystal";
        actions = new float[][]{
            bolt
        };
        actionNames = new string[]{
            action0
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
        List<Unit> sortedUnits = unitMenuController.GetUnits()
            .OrderBy(unit => gridManager.GetManhattan(currentTilePos, unit.currentTilePos))
            .ToList();
        Unit closestUnit = this;
        float lowestHealthFound = 999;
        foreach (Unit unit in sortedUnits){
            if (gridManager.GetManhattan(unit.currentTilePos, currentTilePos) > 3){
                break;
            }
            if(unit.currentHealth < lowestHealthFound){
                closestUnit = unit;
                lowestHealthFound = unit.currentHealth;
            }
        }
        if (closestUnit == this){
            return;
        } else{
            CombatManager.Instance.HandleCombat(this, closestUnit, actions[0], actionNames[0], gridManager.GetManhattan(closestUnit.currentTilePos, currentTilePos));
        }
    }
}
