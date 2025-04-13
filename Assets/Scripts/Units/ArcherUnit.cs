using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUnit : Unit
{   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PerformAction()
    {
        base.PerformAction();

        // Implement Archer-specific action logic here
        Debug.Log("Archer is performing an action!");
    }
}
