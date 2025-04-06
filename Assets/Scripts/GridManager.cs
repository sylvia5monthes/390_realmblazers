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
    private HashSet<Vector3Int> highlightedMoveTiles = new HashSet<Vector3Int>();
    private Unit unitWaitingToMove = null;


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
        Vector3Int gridPos = WorldPositionToGridPosition(tilemap.WorldToCell(worldPos));

        if (!IsInBounds(gridPos)) return;

        // ✅ If a move is pending and the clicked tile is valid:
        if (unitWaitingToMove != null && highlightedMoveTiles.Contains(gridPos))
        {
            MoveUnit(unitWaitingToMove, gridPos);
            unitWaitingToMove.hasMoved = true;
            unitWaitingToMove = null;
            highlightedMoveTiles.Clear();
            highlighter.ClearAllTiles();
            return;
        }

        // Otherwise, normal click handling...
        Tile clickedTile = grid[gridPos.x, gridPos.y];
        if (clickedTile.IsOccupied)
        {
            Unit unit = clickedTile.unitOnTile.GetComponent<Unit>();
            FindObjectOfType<UnitMenuController>()?.ShowMenu(unit);
        }
        else
        {
            HighlightTileAt(gridPos);
        }
    }

    if (Input.GetMouseButtonDown(1))
    {
        Debug.Log("Right mouse button clicked");
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int originalGridPos = tilemap.WorldToCell(worldPos);
        Vector3Int adjustedGridPos = WorldPositionToGridPosition(originalGridPos);

        GameObject newUnit = Instantiate(unitPrefab);
        PlaceUnitOnTile(newUnit, adjustedGridPos);
    }
}

    bool IsInBounds(Vector3Int pos)
    {
        Debug.Log($"Checking bounds for {pos}");
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public void PlaceUnitOnTile(GameObject unit, Vector3Int gridPos)
    {
        if (IsInBounds(gridPos))
        {
            Tile tile = grid[gridPos.x, gridPos.y];
            if (!tile.IsOccupied)
            {
                Debug.Log($"Placing unit on tile at {gridPos}");
                tile.unitOnTile = unit;
                unit.transform.position = tilemap.GetCellCenterWorld(GridPositionToWorldPosition(gridPos));

                // Optionally update the unit's own reference to its grid position
                Unit unitScript = unit.GetComponent<Unit>();
                if (unitScript != null)
                {
                    unitScript.currentTilePos = gridPos;
                }
            } else 
            {
                Debug.Log($"Tile at {gridPos} is already occupied by another unit.");
            }
        } else 
        {
            Debug.Log($"Grid position {gridPos} is out of bounds.");
        }
    }

    public void RemoveUnit(Vector3Int gridPos)
    {
        if (IsInBounds(gridPos))
        {
            grid[gridPos.x, gridPos.y].unitOnTile = null;
        }
    }

    // takes in the adjusted grid position
    // and highlights the tile at that position
    // in the world space
    void HighlightTileAt(Vector3Int gridPos)
    {
        highlighter.ClearAllTiles(); // Remove old highlights
        highlighter.SetTile(GridPositionToWorldPosition(gridPos), highlightedTile);
    }

    // takes in the adjusted grid position
    // and highlights all the tiles that are within
    // the range of the unit on that tile
    // in the world space
    public void HighlightMovableTiles(Vector3Int gridPos){
        highlighter.ClearAllTiles();
        highlightedMoveTiles.Clear();
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

                        if (IsInBounds(pos) && !grid[pos.x, pos.y].IsOccupied && !pos.Equals(gridPos))
                        {
                            highlighter.SetTile(GridPositionToWorldPosition(pos), movableTile);
                            highlightedMoveTiles.Add(pos);
                        }
                    }
                }
            }
        }

    }

    // converts position from the world space, where (0,0) is centered
    // to our grid space, where bottom left corner is (0,0)
        public Vector3Int WorldPositionToGridPosition(Vector3 originalGridPos)
    {
        float offset_x = width / 2;
        float offset_y = height / 2;
        Vector3Int adjustedPos = new Vector3Int((int)(originalGridPos.x + offset_x), (int)(originalGridPos.y + offset_y), (int)originalGridPos.z);

        Debug.Log($"Original Grid Position: {originalGridPos}, Adjusted Position: {adjustedPos}");

        return adjustedPos;
    }


    // converts position from the grid space, where bottom left corner is (0,0)
    // to our world space, where (0,0) is centered
    public Vector3Int GridPositionToWorldPosition(Vector3Int adjustedGridPos)
    {
        float offset_x = width / 2;
        float offset_y = height / 2;
        Vector3Int originalGridPos = new Vector3Int(adjustedGridPos.x - (int)offset_x, adjustedGridPos.y - (int)offset_y, (int)adjustedGridPos.z);

        Debug.Log($"Adjusted Grid Position: {adjustedGridPos}, Original Position: {originalGridPos}");

        return originalGridPos;
    }
    public void StartMove(Unit unit)
    {
        unitWaitingToMove = unit;
    }

    private void MoveUnit(Unit unit, Vector3Int newGridPos)
    {
        Vector3Int oldGridPos = unit.currentTilePos;

        // Update grid references
        RemoveUnit(oldGridPos);
        PlaceUnitOnTile(unit.gameObject, newGridPos);

        Debug.Log($"Moved {unit.name} to {newGridPos}");
    }

}
