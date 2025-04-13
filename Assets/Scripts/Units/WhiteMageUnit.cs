using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteMageUnit : Unit
{   
    float[] holy = new float[] {1, 5, 0.9f, 1}; //index 1: physical vs magic. 1 is magic, 0 is phys. then: power, accuracy, and range
    string action0 = "Holy";
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "White Mage";
        actions = new float[][]{
            holy
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
        
        // Implement Knight-specific action logic here
        Debug.Log("Knight is performing an action!");
    }
}
