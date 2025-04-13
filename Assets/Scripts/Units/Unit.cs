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
    public bool hasAttacked = false;
    public float currentHealth;

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
    public virtual void PerformAction() {

        // add generic action logic here
    }
}
