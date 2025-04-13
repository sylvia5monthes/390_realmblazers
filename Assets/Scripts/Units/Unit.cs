using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Unit : MonoBehaviour
{
    public string unitDisplayName = "";
    public bool isEnemy = false;

    // stats fields
    public float health;
    public float atk;
    public float def;
    public float matk;
    public float mdef;
    public float prec;
    public float eva;
    public int mov = 1;


    // current state fields
    public Vector3Int currentTilePos;
    public bool hasMoved = false;
    public bool hasActed = false;
    public float currentHealth;
    public float[][] actions;
    public string[] actionNames;

    protected virtual void Start()
    {
        currentHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo(Vector3Int newTilePos)
    {
        currentTilePos = newTilePos;
        transform.position = FindObjectOfType<Tilemap>().GetCellCenterWorld(newTilePos);
    }

    void OnMouseDown()
    {   
        if (GameManager.Instance.currentPhase != GameManager.GamePhase.PlayerPhase ||
            GameManager.Instance.isPhaseChanging)
        {
            Debug.Log("Not your turn!");
            return;
        }
        Debug.Log($"Unit {gameObject.name} was clicked!");
        UnitMenuController actionMenu = FindObjectOfType<UnitMenuController>();
        if (actionMenu != null)
        {
            actionMenu.ShowUnitActionMenu(this);
        }
    }

    // can be overriden by specific unit classes
    public virtual void PerformAction(int index) {

        // add generic action logic here
    }//redefining in below UseAbility code so that i don't have to manually update every script- we might be able to repurpose this (ie, items?)
    //the point is i want the child/specific unit class to be able to call it with the index but pass in the actual float itself here
    //we can also restructure this later if this is a mess
    public void UseAbility(float[] ability, Unit unit){

    }
    public void UseAbility(float[] ability){//overloading? we might not need this but we'll probably have targeted and non-targeted abilities

    }
}
