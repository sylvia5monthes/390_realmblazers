using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackMageUnit : Unit
{   
    float[] fire = new float[] {1, 6, 0.9f, 1}; //index 1: physical vs magic. 1 is magic, 0 is phys. then: power, accuracy, and range
    string action0 = "Fire";
    float[] fireball = new float[] {1, 3, 0.75f, 2};
    string action1 = "Fireball";
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitDisplayName = "Black Mage";
        int level = GetLevel();
        if (level >=1){
            actions = new float[][]{
                fire, fireball
            };
            actionNames = new string[]{
                action0, action1
            };
        } else{
            actions = new float[][]{
                fire
            };
            actionNames = new string[]{
                action0
            };
        }
        health += level * 4;
        currentHealth = health;
        atk += level * 2;
        def += level * 3;
        matk += level * 4;
        mdef += level * 3;
        prec += level * 3;
        eva += level * 3;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PerformAction(int index)
    {
        base.PerformAction(index);

        // Implement Archer-specific action logic here
        Debug.Log("Black Mage is performing an action!");
    }
}
