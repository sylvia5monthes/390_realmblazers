using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
