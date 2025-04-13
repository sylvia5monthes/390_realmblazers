using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceDemonEnemyUnit : Unit
{   
    float[] smash = new float[] {0, 5, 0.9f, 1}; //index 1: physical vs magic. 1 is magic, 0 is phys. then: power, accuracy, and range
    string action0 = "Smash";   
    float[] blizzard = new float[]{1, 5, 0.8f,1};
    string action1 = "Blizzard";
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "Ice Demon";
        actions = new float[][]{
            smash, blizzard
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

        // Implement Archer-specific action logic here
        Debug.Log("Ice Demon is performing an action!");
    }
}
