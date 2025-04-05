using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap highlighter;
    public TileBase highlightedTile;
    public TileBase movableTile;
    public int width = 10;
    public int height = 10;
    public GameObject unitPrefab;

    private Tile[,] grid;

    void Start()
    {
        grid = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                grid[x, y] = new Tile(cellPos);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = tilemap.WorldToCell(worldPos);

            if (IsInBounds(gridPos))
            {
                Tile tile = grid[gridPos.x, gridPos.y];
                Debug.Log($"Clicked tile at {gridPos}. Occupied? {tile.IsOccupied}");
            }
            HighlightTileAt(gridPos);
            HighLightMovableTiles(gridPos);
        }
        if (Input.GetMouseButtonDown(1))
    {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = tilemap.WorldToCell(worldPos);

            GameObject newUnit = Instantiate(unitPrefab);
            PlaceUnitOnTile(newUnit, gridPos);
        }
    }

    bool IsInBounds(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public void PlaceUnitOnTile(GameObject unit, Vector3Int gridPos)
    {
        if (IsInBounds(gridPos))
        {
            Tile tile = grid[gridPos.x, gridPos.y];
            if (!tile.IsOccupied)
            {
                tile.unitOnTile = unit;
                unit.transform.position = tilemap.GetCellCenterWorld(gridPos);

                // Optionally update the unit's own reference to its grid position
                Unit unitScript = unit.GetComponent<Unit>();
                if (unitScript != null)
                {
                    unitScript.currentTilePos = gridPos;
                }
            }
        }
    }

    public void RemoveUnit(Vector3Int gridPos)
    {
        if (IsInBounds(gridPos))
        {
            grid[gridPos.x, gridPos.y].unitOnTile = null;
        }
    }

    void HighlightTileAt(Vector3Int gridPos)
    {
        highlighter.ClearAllTiles(); // Remove old highlights
        highlighter.SetTile(gridPos, highlightedTile);
    }

    void HighLightMovableTiles(Vector3Int gridPos){
        Tile tile = grid[gridPos.x, gridPos.y];
        if (tile.IsOccupied){
            GameObject unitObject = tile.unitOnTile;
            Unit unitScript = unitObject.GetComponent<Unit>();
            int range = unitScript.mov;
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    // Manhattan distance check
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) <= range)
                    {
                        Vector3Int pos = new Vector3Int(gridPos.x + dx, gridPos.y + dy, 0);

                        if (IsInBounds(pos) && !grid[pos.x, pos.y].IsOccupied)
                        {
                            if (!pos.Equals(gridPos)){
                                highlighter.SetTile(pos, movableTile);
                            }
                            
                        }
                    }
                }
            }
        }

    }

}
