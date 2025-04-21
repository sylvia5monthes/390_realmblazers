using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
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
        int level = GameManager.Instance.currentLevel-1;
        health += level * 5;
        currentHealth = health;
        atk += level * 4;
        def += level * 3;
        matk += level * 3;
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
        
        // Implement Knight-specific action logic here
        Debug.Log("Knight is performing an action!");
    }
}
