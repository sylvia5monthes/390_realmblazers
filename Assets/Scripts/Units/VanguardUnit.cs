using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanguardUnit : Unit
{   
    float[] crush = new float[] {0, 4, 0.9f, 1}; //index 1: physical vs magic. 1 is magic, 0 is phys. then: power, accuracy, and range
    string action0 = "Crush";
    float[] protect = new float[] {3, 0, 1f, 2};
    string action1 = "Protect";
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "Vanguard";
        int level = GetLevel();
        if (level >=1){
            actions = new float[][]{
                crush, protect
            };
            actionNames = new string[]{
                action0, action1
            };
        } else{
            actions = new float[][]{
                crush
            };
            actionNames = new string[]{
                action0
            };
        }
        health += level * 6;
        currentHealth = health;
        atk += level * 3;
        def += level * 4;
        matk += level * 2;
        mdef += level * 3;
        prec += level * 2;
        eva += level * 2;
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
