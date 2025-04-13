using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnemyUnit : Unit
{   

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "Goblin";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PerformAction()
    {
        base.PerformAction();

        // Implement Archer-specific action logic here
        Debug.Log("Goblin is performing an action!");
    }
}
