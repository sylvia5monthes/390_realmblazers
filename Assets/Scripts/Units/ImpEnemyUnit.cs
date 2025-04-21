using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpEnemyUnit : Unit
{   
    float[] spark = new float[] {1, 7, 0.9f, 2}; //index 1: physical vs magic. 1 is magic, 0 is phys. then: power, accuracy, and range
    string action0 = "Spark";
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "Imp";
        actions = new float[][]{
            spark
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
