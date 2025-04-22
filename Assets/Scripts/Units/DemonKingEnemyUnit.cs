using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKingEnemyUnit : Unit
{   
    float[] dark = new float[] {1, 7, 0.9f, 1}; //index 1: physical vs magic. 1 is magic, 0 is phys. then: power, accuracy, and range
    string action0 = "Dark";
    float[] smash = new float[] {0, 7, 0.9f, 1};
    string action1 = "Smash";
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "Demon King";
        actions = new float[][]{
            dark
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
}
