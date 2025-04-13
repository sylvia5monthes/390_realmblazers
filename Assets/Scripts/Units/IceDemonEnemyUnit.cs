using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceDemonEnemyUnit : Unit
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
        Debug.Log("Ice Demon is performing an action!");
    }
}
