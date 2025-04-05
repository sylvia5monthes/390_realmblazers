using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector3Int gridPosition;
    public GameObject unitOnTile;

    public bool IsOccupied => unitOnTile != null;

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
