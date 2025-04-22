using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellhoundEnemyUnit : Unit
{   
    float[] bite = new float[] {0, 7, 0.85f, 1}; //index 1: physical vs magic. 1 is magic, 0 is phys. then: power, accuracy, and range
    string action0 = "Bite";
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "Hellhound";
        actions = new float[][]{
            bite
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

    }
}
