using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Unit : MonoBehaviour
{
    // Start is called before the first frame update
    public float health;
    public float atk;
    public float def;
    public float matk;
    public float mdef;
    public float prec;
    public float eva;
    public int mov = 1;
    public Vector3Int currentTilePos;
    void Start()
    {
        
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
}
