using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Vector3Int gridPosition;
    public GameObject unitOnTile;

    public bool IsOccupied => unitOnTile != null;

    public bool isTerrain = false;

    public bool isBrush = false;
    public bool isMagma = false;

    public Tile(Vector3Int position)
    {
        gridPosition = position;
        unitOnTile = null;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
