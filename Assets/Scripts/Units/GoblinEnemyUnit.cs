using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnemyUnit : Unit
{   
    float[] pound = new float[] {0, 7, 0.9f, 1}; //index 1: physical vs magic. 1 is magic, 0 is phys. then: power, accuracy, and range
    string action0 = "Pound";
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "Goblin";
        actions = new float[][]{
            pound
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

        // Implement Archer-specific action logic here
        Debug.Log("Goblin is performing an action!");
    }
}
