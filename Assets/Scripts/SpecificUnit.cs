using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificUnit : Unit
{   
    public float specificUnitHealth;
    public float specificUnitAtk;
    public float specificUnitDef;
    public float specificUnitMatk;
    public float specificUnitMdef;
    public float specificUnitPrec;
    public float specificUnitEva;
    public int specificUnitMov;

    // Start is called before the first frame update
    void Start()
    {
        // initialize base class properties
        health = specificUnitHealth;
        atk = specificUnitAtk;
        def = specificUnitDef;
        matk = specificUnitMatk;
        mdef = specificUnitMdef;
        prec = specificUnitPrec;
        eva = specificUnitEva;
        mov = specificUnitMov;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
