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
    private GameObject selectedUnitPrefab;

    private Tile[,] grid;
    private HashSet<Vector3Int> highlightedMoveTiles = new HashSet<Vector3Int>();
    private Unit unitWaitingToMove = null;
    private Unit unitWaitingToAct = null;
    private float[] pendingAction = null;

    // spawn enemies
    // TODO: make this dynamic for each level
    public GameObject goblinPrefab;
    private UnitMenuController unitMenuController;
    private EnemyController enemyController;
    
    void Start()
    {
        enemyController = FindObjectOfType<EnemyController>();
        grid = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                grid[x, y] = new Tile(cellPos);
            }
        }

        // Spawn enemies at the start
        SpawnEnemiesAtStart();
        unitMenuController = FindObjectOfType<UnitMenuController>();
    }

void Update()
{
    if (Input.GetMouseButtonDown(0) && !GameManager.Instance.isPhaseChanging)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = WorldPositionToGridPosition(tilemap.WorldToCell(worldPos));

        if (!IsInBounds(gridPos)) return;

        // if a move is pending and the clicked tile is valid:
        if (unitWaitingToMove != null && highlightedMoveTiles.Contains(gridPos))
        {
            MoveUnit(unitWaitingToMove, gridPos);
            //unitWaitingToMove.hasMoved = true;
            unitMenuController.UnitHasMoved(unitWaitingToMove);//this does above line
            unitWaitingToMove = null;
            highlightedMoveTiles.Clear();
            highlighter.ClearAllTiles();
            return;
        }
        // if a action is pending and the clicked tile is valid:
        if (unitWaitingToAct != null && highlightedMoveTiles.Contains(gridPos))
        {
            if(pendingAction[0] == 2){
                CombatManager.Instance.HandleHeal(unitWaitingToAct,grid[gridPos.x, gridPos.y].unitOnTile.GetComponent<Unit>(), pendingAction);
                unitMenuController.UnitHasActed(unitWaitingToAct);
                unitWaitingToAct = null;
                pendingAction = null;
                highlightedMoveTiles.Clear();
                highlighter.ClearAllTiles();
                return;
            } else{
                Vector3Int waitingPos = unitWaitingToAct.currentTilePos;
                int distance = Mathf.Abs(gridPos.x-waitingPos.x) + Mathf.Abs(gridPos.y - waitingPos.y);
                CombatManager.Instance.HandleCombat(unitWaitingToAct,grid[gridPos.x, gridPos.y].unitOnTile.GetComponent<Unit>(), pendingAction, distance);
                unitMenuController.UnitHasActed(unitWaitingToAct);
                unitWaitingToAct = null;
                pendingAction = null;
                highlightedMoveTiles.Clear();
                highlighter.ClearAllTiles();
                return;
            }
        }

        // Otherwise, normal click handling...
        Tile clickedTile = grid[gridPos.x, gridPos.y];
        if (clickedTile.IsOccupied)
        {
            Unit unit = clickedTile.unitOnTile.GetComponent<Unit>();
            //FindObjectOfType<UnitMenuController>()?.ShowMenu(unit);
        }
        HighlightTileAt(gridPos);
    }

    if (Input.GetMouseButtonDown(1) && !GameManager.Instance.isPhaseChanging)
    {
        Debug.Log("Right mouse button clicked");
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int originalGridPos = tilemap.WorldToCell(worldPos);
        Vector3Int adjustedGridPos = WorldPositionToGridPosition(originalGridPos);

        if (selectedUnitPrefab != null && IsInBounds(adjustedGridPos))
        {
            GameObject unit = Instantiate(selectedUnitPrefab);
            // try to place unit; if fail, delete it
            if (!PlaceUnitOnTile(unit, adjustedGridPos))
            {
                Destroy(unit);
                return;
            }
            FindObjectOfType<UnitMenuController>()?.MarkUnitAsPlaced(selectedUnitPrefab);
            selectedUnitPrefab = null; // Reset the selected unit prefab after placing
        }
    }
}

    public bool IsInBounds(Vector3Int pos)
    {
        Debug.Log($"Checking bounds for {pos}");
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
    public Tile TileAt(Vector3Int gridPos){
        return grid[gridPos.x, gridPos.y];
    }

    public bool PlaceUnitOnTile(GameObject unit, Vector3Int gridPos)
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
                return false; // Tile is occupied
            }
        } else 
        {
            Debug.Log($"Grid position {gridPos} is out of bounds.");
            return false; // Out of bounds
        }
        return true; // Successfully placed the unit
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
            
            List<Vector3Int> movableTiles = GetMovableTiles(gridPos, range);
            foreach (Vector3Int pos in movableTiles)
            {
                highlighter.SetTile(GridPositionToWorldPosition(pos), movableTile);
                highlightedMoveTiles.Add(pos);
            }
        }

    }
    public List<Vector3Int> GetMovableTiles(Vector3Int gridPos, int range, bool isEnemy = false){
        // pathfinding w/ bfs, can only move through ally tiles
        List<Vector3Int> ret = new List<Vector3Int>();
        Queue<(Vector3Int pos, int cost)> queue = new Queue<(Vector3Int, int)>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        Unit movingUnit = grid[gridPos.x, gridPos.y].unitOnTile.GetComponent<Unit>();

        queue.Enqueue((gridPos, 0));
        visited.Add(gridPos);

        // 4 cardinal directions
        Vector3Int[] directions = new Vector3Int[] {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0)
        };

        while (queue.Count > 0) {
            var (currentPos, currentCost) = queue.Dequeue();

            foreach (var dir in directions) {
                Vector3Int nextPos = currentPos + dir; 

                if (!IsInBounds(nextPos) || visited.Contains(nextPos)) {
                    continue; // Skip out of bounds or already visited
                }

                int nextCost = currentCost + 1;
                if (nextCost > range) {
                    continue; // Skip if cost exceeds range
                }

                Tile tile = grid[nextPos.x, nextPos.y];

                if (!tile.IsOccupied) {
                    ret.Add(nextPos);
                    queue.Enqueue((nextPos, nextCost));
                    visited.Add(nextPos);
                } else {
                    // if occupied, check if it's an ally or enemy
                    Unit unitOnTile = tile.unitOnTile.GetComponent<Unit>();

                    // ok to walk through if an ally
                    if (unitOnTile.isEnemy == movingUnit.isEnemy){
                        queue.Enqueue((nextPos, nextCost));
                        visited.Add(nextPos);
                    }
                }
            }
        }

        // Debug.Log($"Movable tiles for {movingUnit.unitDisplayName} at {gridPos}:");
        // foreach (var pos in ret) {
        //     Debug.Log(pos);
        // }
        return ret;
    }

    public void HighlightActableTiles(Vector3Int gridPos, int range){//again, currently only for combat abilities (enemy units)
        highlighter.ClearAllTiles();
        highlightedMoveTiles.Clear();
        Tile tile = grid[gridPos.x, gridPos.y];
        if (tile.IsOccupied){
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    // Manhattan distance check
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) <= range)
                    {
                        Vector3Int pos = new Vector3Int(gridPos.x + dx, gridPos.y + dy, 0);

                        if (IsInBounds(pos) && grid[pos.x, pos.y].IsOccupied && !pos.Equals(gridPos) && grid[pos.x, pos.y].unitOnTile.GetComponent<Unit>().isEnemy)
                        {
                            highlighter.SetTile(GridPositionToWorldPosition(pos), movableTile);
                            highlightedMoveTiles.Add(pos);
                        }
                    }
                }
            }
        }
    }
    public void HighlightAlliedTiles(Vector3Int gridPos, int range){//again, currently only for combat abilities (enemy units)
        highlighter.ClearAllTiles();
        highlightedMoveTiles.Clear();
        Tile tile = grid[gridPos.x, gridPos.y];
        if (tile.IsOccupied){
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    // Manhattan distance check
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) <= range)
                    {
                        Vector3Int pos = new Vector3Int(gridPos.x + dx, gridPos.y + dy, 0);

                        if (IsInBounds(pos) && grid[pos.x, pos.y].IsOccupied && !pos.Equals(gridPos) && !grid[pos.x, pos.y].unitOnTile.GetComponent<Unit>().isEnemy)
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
        HighlightMovableTiles(unit.currentTilePos);
    }
    public void StartAction(Unit unit, float[] action)
    {
        unitWaitingToAct = unit;
        pendingAction = action;
        if (action[0] == 2){
            HighlightAlliedTiles(unit.currentTilePos, (int)action[3]);
        } else{
            HighlightActableTiles(unit.currentTilePos, (int)action[3]);
        }
        //range is the 3rd entry in the action
    }

    private void MoveUnit(Unit unit, Vector3Int newGridPos)
    {
        Vector3Int oldGridPos = unit.currentTilePos;

        // Update grid references
        RemoveUnit(oldGridPos);
        PlaceUnitOnTile(unit.gameObject, newGridPos);

        Debug.Log($"Moved {unit.name} to {newGridPos}");
    }
    public void MoveEnemy(Unit unit, Vector3Int newGridPos){
        MoveUnit(unit, newGridPos);
    }

    public void SetSelectedUnitPrefab(GameObject unitPrefab)
    {
        selectedUnitPrefab = unitPrefab;
    }

    public void SpawnEnemiesAtStart()
    {
        List<Vector3Int> enemySpawnPositions = new List<Vector3Int>
        {
            new Vector3Int(width - 1, 0, 0),
            new Vector3Int(width - 1, 1, 0),
            new Vector3Int(width - 2, 0, 0),
        };

        foreach (var adjustedGridPos in enemySpawnPositions)
        {
            GameObject enemyUnit = Instantiate(goblinPrefab);
            Unit enemyUnitScript = enemyUnit.GetComponent<Unit>();
            enemyController.ActivateEnemy(enemyUnitScript);
            PlaceUnitOnTile(enemyUnit, adjustedGridPos);
        }
    }
    public int GetManhattan(Vector3Int tilePos1, Vector3Int tilePos2){
        return Mathf.Abs(tilePos1.x - tilePos2.x) + Mathf.Abs(tilePos1.y - tilePos2.y);
    }

}
