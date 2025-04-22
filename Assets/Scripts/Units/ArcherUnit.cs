using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUnit : Unit
{   
    float[] shoot = new float[] {0, 5, 0.95f, 2}; //index 1: physical vs magic. 1 is magic, 0 is phys. then: power, accuracy, and range
    string action0 = "Shoot";
    float[] snipe = new float[] {0, 2, 0.85f, 3};
    string action1 = "Snipe";
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "Archer";
        int level = GetLevel();
        if (level >=1){
            actions = new float[][]{
                shoot, snipe
            };
            actionNames = new string[]{
                action0, action1
            };
        } else{
            actions = new float[][]{
                shoot
            };
            actionNames = new string[]{
                action0
            };
        }
        health += level * 4;
        currentHealth = health;
        atk += level * 4;
        def += level * 3;
        matk += level * 3;
        mdef += level * 3;
        prec += level * 4;
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
