using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUnit : Unit
{   
    float[] shoot = new float[] {0, 5, 0.95f, 2}; //index 1: physical vs magic. 1 is magic, 0 is phys. then: power, accuracy, and range
    string action0 = "Shoot";
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "Archer";
        actions = new float[][]{
            shoot
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
        Debug.Log("Archer is performing an action!");
    }
}
