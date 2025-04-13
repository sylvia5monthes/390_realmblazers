using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class KnightUnit : Unit
{   
    //may change structure later
    float[] slash = new float[] {0, 5, 0.9f, 1}; //index 1: physical vs magic. 1 is magic, 0 is phys. then: power, accuracy, and range
    string action0 = "Slash";
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "Knight";
        actions = new float[][]{
            slash
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
        
        // Implement Knight-specific action logic here
        Debug.Log(actionNames[index]);
    }
}
