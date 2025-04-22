using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap highlighter;
    public TileBase highlightedTile;
    public Tilemap placementHighlighter;
    public TileBase placementHighlightedTile;
    public TileBase movableTile;
    public int width = 10;
    public int height = 10;
    public int initialSpawnWidth = 3;
    public GameObject unitPrefab;
    private GameObject selectedUnitPrefab;
    public GameObject snowTerrainPrefab;
    public GameObject snowBrushPrefab;
    public GameObject magmaTerrainPrefab;
    public GameObject magmaBrushPrefab;
    public GameObject magmaDamagePrefab;

    public GameObject darknessTerrainPrefab;
    public GameObject darknessBrushPrefab;
    private Tile[,] grid;
    private HashSet<Vector3Int> highlightedMoveTiles = new HashSet<Vector3Int>();
    private Unit unitWaitingToMove = null;
    private Unit unitWaitingToAct = null;
    private float[] pendingAction = null;
    private string pendingActionName = null;

    // spawn enemies
    public GameObject goblinPrefab;
    public GameObject iceDemonPrefab;
    public GameObject impPrefab;
    public GameObject hellhoundPrefab;
    public GameObject incubusPrefab;
    public GameObject succubusPrefab;
    public GameObject crystalPrefab;
    public GameObject demonPrefab;
    public GameObject warlockPrefab;
    public GameObject demonKingPrefab;
    private UnitMenuController unitMenuController;
    private EnemyController enemyController;
    private int level;
    
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
        unitMenuController = FindObjectOfType<UnitMenuController>();
        level = unitMenuController.level;
        SpawnEnemiesAtStart();
        SpawnTerrainsAtStart();

        // Highlight the initial placement area
        HighlightInitialPlacementArea();
    }

void Update()
{
    // Handle mouse input for unit movement and action selection
    // don't allow clicking through UI
    if (Input.GetMouseButtonDown(0) && !GameManager.Instance.isPhaseChanging && !IsPointerOverUI())
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
        // perform combat
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
            } else if (pendingAction[0] == 3){//protect
                CombatManager.Instance.HandleProtect(unitWaitingToAct,grid[gridPos.x, gridPos.y].unitOnTile.GetComponent<Unit>(), pendingAction);
                unitMenuController.UnitHasActed(unitWaitingToAct);
                unitWaitingToAct = null;
                pendingAction = null;
                highlightedMoveTiles.Clear();
                highlighter.ClearAllTiles();
                return;
            } else{
                Vector3Int waitingPos = unitWaitingToAct.currentTilePos;
                int distance = Mathf.Abs(gridPos.x-waitingPos.x) + Mathf.Abs(gridPos.y - waitingPos.y);
                CombatManager.Instance.HandleCombat(unitWaitingToAct,grid[gridPos.x, gridPos.y].unitOnTile.GetComponent<Unit>(), pendingAction, pendingActionName, distance);
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

    // allow cancelling action/movement if one is pending with the escape key
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        if (unitWaitingToAct != null || unitWaitingToMove != null)
        {
            Debug.Log("Cancelling action");
            unitWaitingToMove = null;
            unitWaitingToAct = null;
            pendingAction = null;
            pendingActionName = null;
            highlightedMoveTiles.Clear();
            highlighter.ClearAllTiles();
        }
    }

    // right click to place unit during initial spawn phase
    if (Input.GetMouseButtonDown(1) && !GameManager.Instance.isPhaseChanging)
    {
        Debug.Log("Right mouse button clicked");
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int originalGridPos = tilemap.WorldToCell(worldPos);
        Vector3Int adjustedGridPos = WorldPositionToGridPosition(originalGridPos);

        // check if the adjusted grid position in in the upper left 3 x 3 corner
        if (selectedUnitPrefab != null && IsInInitialSpawnArea(adjustedGridPos))
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

    private bool IsPointerOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    public bool IsInBounds(Vector3Int pos)
    {
        Debug.Log($"Checking bounds for {pos}");
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public bool IsInInitialSpawnArea(Vector3Int pos)
    {
        // Check if the position is within the initial spawn area (upper left 3x3 corner)
        return pos.x >= 0 && pos.x < initialSpawnWidth && pos.y >= height - initialSpawnWidth && pos.y < height;
    }

    public Tile TileAt(Vector3Int gridPos){
        return grid[gridPos.x, gridPos.y];
    }
    public bool PlaceTerrainOnTile(GameObject terrain, Vector3Int gridPos){
        if (IsInBounds(gridPos))
        {
            Tile tile = grid[gridPos.x, gridPos.y];
            if (!tile.IsOccupied)
            {
                Debug.Log($"Placing terrain on tile at {gridPos}");
                terrain.transform.position = tilemap.GetCellCenterWorld(GridPositionToWorldPosition(gridPos));
                tile.unitOnTile = terrain; //make isoccupied true such that terrain is impassable
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
        return true; // Successfully placed the terrain
    }
    public bool PlaceBrushOnTile(GameObject brush, Vector3Int gridPos){
        if (IsInBounds(gridPos))
        {
            Tile tile = grid[gridPos.x, gridPos.y];
            if (!tile.IsOccupied)
            {
                Debug.Log($"Placing terrain on tile at {gridPos}");
                brush.transform.position = tilemap.GetCellCenterWorld(GridPositionToWorldPosition(gridPos));
                tile.isBrush = true;
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
        return true; // Successfully placed the terrain
    }
    public bool PlaceMagmaOnTile(GameObject magma, Vector3Int gridPos){
        if (IsInBounds(gridPos))
        {
            Tile tile = grid[gridPos.x, gridPos.y];
            if (!tile.IsOccupied)
            {
                Debug.Log($"Placing terrain on tile at {gridPos}");
                magma.transform.position = tilemap.GetCellCenterWorld(GridPositionToWorldPosition(gridPos));
                tile.isMagma = true;
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
        return true; // Successfully placed the terrain
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

    public void HighlightInitialPlacementArea()
    {
        placementHighlighter.ClearAllTiles();

        for (int x = 0; x < initialSpawnWidth; x++)
        {
            for (int y = height - initialSpawnWidth; y < height; y++)
            {
                Vector3Int gridPos = new Vector3Int(x, y, 0);
                placementHighlighter.SetTile(GridPositionToWorldPosition(gridPos), placementHighlightedTile);
            }
        }
    }

    public void ClearInitialPlacementHighlightedTiles()
    {
        placementHighlighter.ClearAllTiles();
    }

    // use pathfinding algo (bfs) to find all the tiles
    // that are within the range of the unit
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
                    if (unitOnTile != null){
                        // ok to walk through if an ally
                        if (unitOnTile.isEnemy == movingUnit.isEnemy){
                            queue.Enqueue((nextPos, nextCost));
                            visited.Add(nextPos);
                        }
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

    // find all the tiles that are within the range of the unit's selected action
    // and highlight them
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
                        if (IsInBounds(pos) && grid[pos.x, pos.y].IsOccupied && !pos.Equals(gridPos)){
                            Unit currentUnitOnTile = grid[pos.x, pos.y].unitOnTile.GetComponent<Unit>();
                            if (currentUnitOnTile != null && currentUnitOnTile.isEnemy)
                            {
                                highlighter.SetTile(GridPositionToWorldPosition(pos), movableTile);
                                highlightedMoveTiles.Add(pos);
                            }
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
                            if (IsInBounds(pos) && grid[pos.x, pos.y].IsOccupied && !pos.Equals(gridPos)){
                                Unit currentUnitOnTile = grid[pos.x, pos.y].unitOnTile.GetComponent<Unit>();
                                if (currentUnitOnTile != null && !currentUnitOnTile.isEnemy)
                                {
                                    highlighter.SetTile(GridPositionToWorldPosition(pos), movableTile);
                                    highlightedMoveTiles.Add(pos);
                                }
                        }
                    }
                }
            }
        }
    }

    public void HighlightEnemyPathTemporarily(Vector3Int from, Vector3Int to, float duration = 1f) {
        StartCoroutine(HighlightAndClear(from, to, duration));
    }

    public void HighlightEnemyPath(Vector3Int from, Vector3Int to)
    {
        highlighter.ClearAllTiles(); 
        // TODO: maybe add different colored highlighted Tile
        highlighter.SetTile(GridPositionToWorldPosition(from), highlightedTile);
        highlighter.SetTile(GridPositionToWorldPosition(to), highlightedTile);
    }

    private IEnumerator HighlightAndClear(Vector3Int from, Vector3Int to, float duration)
    {
        HighlightEnemyPath(from, to);
        yield return new WaitForSeconds(duration);
        highlighter.ClearAllTiles();
    }


    // converts position from the world space, where (0,0) is centered
    // to our grid space, where bottom left corner is (0,0)
    public Vector3Int WorldPositionToGridPosition(Vector3 originalGridPos)
    {
        float offset_x = width / 2;
        float offset_y = height / 2;
        Vector3Int adjustedPos = new Vector3Int((int)(originalGridPos.x + offset_x), (int)(originalGridPos.y + offset_y), (int)originalGridPos.z);

        //Debug.Log($"Original Grid Position: {originalGridPos}, Adjusted Position: {adjustedPos}");

        return adjustedPos;
    }


    // converts position from the grid space, where bottom left corner is (0,0)
    // to our world space, where (0,0) is centered
    public Vector3Int GridPositionToWorldPosition(Vector3Int adjustedGridPos)
    {
        float offset_x = width / 2;
        float offset_y = height / 2; 
        Vector3Int originalGridPos = new Vector3Int(adjustedGridPos.x - (int)offset_x, adjustedGridPos.y - (int)offset_y, (int)adjustedGridPos.z);

        //Debug.Log($"Adjusted Grid Position: {adjustedGridPos}, Original Position: {originalGridPos}");

        return originalGridPos;
    }
    public void StartMove(Unit unit)
    {
        if (pendingAction != null){
            pendingAction = null;
            unitWaitingToAct = null;
        }
        unitWaitingToMove = unit;
        HighlightMovableTiles(unit.currentTilePos);
    }
    public void StartAction(Unit unit, float[] action, string actionName)
    {
        if (unitWaitingToMove !=null){
            unitWaitingToMove= null;
        }
        unitWaitingToAct = unit;
        pendingAction = action;
        pendingActionName = actionName;
        if (action[0] >= 2){
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

    // for each level, specify where to spawn enemies on the grid
    public void SpawnEnemiesAtStart()
    {
        if (level == 1){
            List<Vector3Int> enemySpawnPositions = new List<Vector3Int>
            {
                new Vector3Int(width - 1, 0, 0),
                new Vector3Int(width - 1, 1, 0),
                new Vector3Int(width - 2, 0, 0),
                new Vector3Int(width-2, 1,0)
            };

            foreach (var adjustedGridPos in enemySpawnPositions)
            {
                GameObject enemyUnit = Instantiate(goblinPrefab);
                Unit enemyUnitScript = enemyUnit.GetComponent<Unit>();
                enemyController.ActivateEnemy(enemyUnitScript);
                PlaceUnitOnTile(enemyUnit, adjustedGridPos);
            }
        } else if (level == 2){
                List<Vector3Int> impSpawnPositions = new List<Vector3Int>
            {
                new Vector3Int(width - 4, 0, 0),
                new Vector3Int(width - 3, 1, 0),
                new Vector3Int(width - 2, 2, 0),
                new Vector3Int(width - 1, 3, 0)
            };

            foreach (var adjustedGridPos in impSpawnPositions)
            {
                GameObject enemyUnit = Instantiate(impPrefab);
                Unit enemyUnitScript = enemyUnit.GetComponent<Unit>();
                enemyController.ActivateEnemy(enemyUnitScript);
                PlaceUnitOnTile(enemyUnit, adjustedGridPos);
            }
            List<Vector3Int> hellhoundSpawnPositions = new List<Vector3Int>
            {
                new Vector3Int(width - 3, 0, 0),
                new Vector3Int(width - 1, 2, 0)
            };

            foreach (var adjustedGridPos in hellhoundSpawnPositions)
            {
                GameObject enemyUnit = Instantiate(hellhoundPrefab);
                Unit enemyUnitScript = enemyUnit.GetComponent<Unit>();
                enemyController.ActivateEnemy(enemyUnitScript);
                PlaceUnitOnTile(enemyUnit, adjustedGridPos);
            }
        } else{
            List<Vector3Int> crystalSpawnPositions = new List<Vector3Int>
            {
                new Vector3Int(3, 3, 0),
                new Vector3Int(8, 8, 0),
                new Vector3Int(8, 3, 0)
            };

            foreach (var adjustedGridPos in crystalSpawnPositions)
            {
                GameObject enemyUnit = Instantiate(crystalPrefab);
                Unit enemyUnitScript = enemyUnit.GetComponent<Unit>();
                enemyController.ActivateEnemy(enemyUnitScript);
                PlaceUnitOnTile(enemyUnit, adjustedGridPos);
            }
            List<Vector3Int> demonSpawnPositions = new List<Vector3Int>
            {
                new Vector3Int(0, 1, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(11, 10, 0),
                new Vector3Int(10, 11, 0)
            };

            foreach (var adjustedGridPos in demonSpawnPositions)
            {
                GameObject enemyUnit = Instantiate(demonPrefab);
                Unit enemyUnitScript = enemyUnit.GetComponent<Unit>();
                enemyController.ActivateEnemy(enemyUnitScript);
                PlaceUnitOnTile(enemyUnit, adjustedGridPos);
            }
            List<Vector3Int> warlockSpawnPositions = new List<Vector3Int>
            {
                new Vector3Int(10, 0, 0),
                new Vector3Int(11, 1, 0),
                new Vector3Int(11,0,0)
            };

            foreach (var adjustedGridPos in warlockSpawnPositions)
            {
                GameObject enemyUnit = Instantiate(warlockPrefab);
                Unit enemyUnitScript = enemyUnit.GetComponent<Unit>();
                enemyController.ActivateEnemy(enemyUnitScript);
                PlaceUnitOnTile(enemyUnit, adjustedGridPos);
            }
        }
    

    }
    public void SpawnBoss()
    {
        int level = unitMenuController.level;
        Debug.Log("[GridManager] Spawning boss unit.");
        if (level == 3){
            GameObject demonKingUnit = Instantiate(demonKingPrefab);
            Unit demonKingUnitScript = demonKingUnit.GetComponent<Unit>();
            enemyController.ActivateEnemy(demonKingUnitScript);
            Vector3Int firstUnoccupied = new Vector3Int(0,0,0);
            List<Vector3Int> demonKingSpawnPositions = new List<Vector3Int>{
                new Vector3Int(10, 0, 0),
                new Vector3Int(10, 1, 0),
                new Vector3Int(11, 0, 0),
                new Vector3Int(11, 1, 0),
                new Vector3Int(9, 1, 0),
                new Vector3Int(10, 2, 0)
            };
            foreach(var adjustedGridPos in demonKingSpawnPositions){
                if(!TileAt(adjustedGridPos).IsOccupied){
                    firstUnoccupied = adjustedGridPos;
                    break;
                }
            }
            PlaceUnitOnTile(demonKingUnit, firstUnoccupied);
            GameObject warlockUnit = Instantiate(warlockPrefab);
            Unit warlockUnitScript = warlockUnit.GetComponent<Unit>();
            enemyController.ActivateEnemy(warlockUnitScript);
            List<Vector3Int> warlockSpawnPositions = new List<Vector3Int>{
                new Vector3Int(1, 2, 0),
                new Vector3Int(2, 1, 0),
                new Vector3Int(0, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(1, 1, 0)
            };
            foreach(var adjustedGridPos in warlockSpawnPositions){
                if(!TileAt(adjustedGridPos).IsOccupied){
                    firstUnoccupied = adjustedGridPos;
                    break;
                }
            }
            PlaceUnitOnTile(warlockUnit, firstUnoccupied);
            GameObject demonUnit = Instantiate(demonPrefab);
            Unit demonUnitScript = demonUnit.GetComponent<Unit>();
            enemyController.ActivateEnemy(demonUnitScript);
            List<Vector3Int> demonSpawnPositions = new List<Vector3Int>{
                new Vector3Int(10, 9, 0),
                new Vector3Int(9, 10, 0),
                new Vector3Int(10, 10, 0),
                new Vector3Int(11, 11, 0),
                new Vector3Int(11, 10, 0),
                new Vector3Int(10, 11, 0)
            };
            foreach(var adjustedGridPos in demonSpawnPositions){
                if(!TileAt(adjustedGridPos).IsOccupied){
                    firstUnoccupied = adjustedGridPos;
                    break;
                }
            }
            PlaceUnitOnTile(demonUnit, firstUnoccupied);
        } else if (level == 2){
            GameObject incubusUnit = Instantiate(incubusPrefab);
             Unit incubusUnitScript = incubusUnit.GetComponent<Unit>();
             enemyController.ActivateEnemy(incubusUnitScript);
             Vector3Int firstUnoccupied = new Vector3Int(0,0,0);
             List<Vector3Int> incubusSpawnPositions = new List<Vector3Int>{
                new Vector3Int(0, 0, 0),
                 new Vector3Int(0, 1, 0),
                 new Vector3Int(1, 0, 0),
                 new Vector3Int(1, 1, 0),
                 new Vector3Int(2,2,0)
             };
             foreach(var adjustedGridPos in incubusSpawnPositions){
                 if(!TileAt(adjustedGridPos).IsOccupied){
                     firstUnoccupied = adjustedGridPos;
                     break;
                 }
             }
             PlaceUnitOnTile(incubusUnit, firstUnoccupied);
             GameObject succubusUnit = Instantiate(succubusPrefab);
             Unit succubusUnitScript = succubusUnit.GetComponent<Unit>();
             enemyController.ActivateEnemy(succubusUnitScript);
             List<Vector3Int> succubusSpawnPositions = new List<Vector3Int>{
                 new Vector3Int(width-1, height-1, 0),
                 new Vector3Int(width-1, height-2, 0),
                 new Vector3Int(width-2, height-1, 0),
                 new Vector3Int(width-2,height-2, 0),
                 new Vector3Int(width-3,height-3,0)
             };
             foreach(var adjustedGridPos in succubusSpawnPositions){
                if(!TileAt(adjustedGridPos).IsOccupied){
                     firstUnoccupied = adjustedGridPos;
                     break;
                 }
             }
             PlaceUnitOnTile(succubusUnit, firstUnoccupied);
        } else{
            GameObject bossUnit = Instantiate(iceDemonPrefab);
            Unit bossUnitScript = bossUnit.GetComponent<Unit>();
            
            enemyController.ActivateEnemy(bossUnitScript);
            List<Vector3Int> spawnPositions = new List<Vector3Int>{
                new Vector3Int(width - 2, 1, 0),
                new Vector3Int(width - 1, 1, 0),
                new Vector3Int(width - 1, 0, 0),
                new Vector3Int(width - 2, 10, 0)
            };
            Vector3Int firstUnoccupied = new Vector3Int(0,0,0);
            foreach(var adjustedGridPos in spawnPositions){
                if(!TileAt(adjustedGridPos).IsOccupied){
                    firstUnoccupied = adjustedGridPos;
                    break;
                }
            }
            PlaceUnitOnTile(bossUnit, firstUnoccupied);
        }
    }
    public void SpawnTerrainsAtStart()
    {
        int level = unitMenuController.level;
        if (level == 1){
            // spawn terrain for rocks
            List<Vector3Int> spawnTerrainPositions = new List<Vector3Int>
            {
                new Vector3Int(width/2, height/2, 0),
                new Vector3Int(width/2-1, height/2-1, 0),
                new Vector3Int(width/2, height/2-1, 0),
                new Vector3Int(width/2-1, height/2, 0)
            };
            foreach (var adjustedGridPos in spawnTerrainPositions)
            {
                GameObject terrain = Instantiate(snowTerrainPrefab);
                PlaceTerrainOnTile(terrain, adjustedGridPos);
            }
            List<Vector3Int> spawnBrushPositions = new List<Vector3Int>
            {
                new Vector3Int(0, 0, 0),
                new Vector3Int(1, 1, 0),
                new Vector3Int(2, 2, 0),
                new Vector3Int(3, 3, 0),
                new Vector3Int(6, 6, 0),
                new Vector3Int(7, 7, 0),
                new Vector3Int(8, 8, 0),
                new Vector3Int(9, 9, 0)
            };
            foreach (var adjustedGridPos in spawnBrushPositions)
            {
                GameObject brush = Instantiate(snowBrushPrefab);
                PlaceBrushOnTile(brush, adjustedGridPos);
            }
        }
        else if(level == 2){
            List<Vector3Int> spawnTerrainPositions = new List<Vector3Int>
            {
                new Vector3Int(0, 6, 0),
                new Vector3Int(5, 0, 0),
                new Vector3Int(6, height-1, 0),
                new Vector3Int(width-1, 5, 0),
                new Vector3Int(width-3, height-1, 0),
                new Vector3Int(width-4, height-2, 0),
                new Vector3Int(width-5, height-3, 0),
                new Vector3Int(width-6, height-4, 0),
                new Vector3Int(width-1, height-3, 0),
                new Vector3Int(width-2, height-4, 0),
                new Vector3Int(width-3, height-5, 0),
                new Vector3Int(width-4, height-6, 0)
            };
            foreach (var adjustedGridPos in spawnTerrainPositions)
            {
                GameObject terrain = Instantiate(magmaTerrainPrefab);
                PlaceTerrainOnTile(terrain, adjustedGridPos);
            }
            List<Vector3Int> spawnBrushPositions = new List<Vector3Int>
            {
                new Vector3Int(2, 0, 0),
                new Vector3Int(3, 1, 0),
                new Vector3Int(4, 2, 0),
                new Vector3Int(5, 3, 0),
                new Vector3Int(0, 2, 0),
                new Vector3Int(1, 3, 0),
                new Vector3Int(2, 4, 0),
                new Vector3Int(3, 5, 0)
            };
            foreach (var adjustedGridPos in spawnBrushPositions)
            {
                GameObject brush = Instantiate(magmaBrushPrefab);
                PlaceBrushOnTile(brush, adjustedGridPos);
            }
            List<Vector3Int> spawnMagmaPositions = new List<Vector3Int>
            {
                new Vector3Int(1, 5, 0),
                new Vector3Int(6, 1, 0),
                new Vector3Int(5, height-2, 0),
                new Vector3Int(width-2, 6, 0),
                new Vector3Int(5, height-6, 0),
                new Vector3Int(6, height-7, 0),
                new Vector3Int(4, height-5, 0),
                new Vector3Int(7, height-8, 0),
                new Vector3Int(4, 4, 0),
                new Vector3Int(7, 7, 0)
            };
            foreach (var adjustedGridPos in spawnMagmaPositions)
            {
                GameObject magma = Instantiate(magmaDamagePrefab);
                PlaceMagmaOnTile(magma, adjustedGridPos);
            }
        } else{
            List<Vector3Int> spawnTerrainPositions = new List<Vector3Int>
            {
                new Vector3Int(2, 2, 0),
                new Vector3Int(4, 4, 0),
                new Vector3Int(2, 4, 0),
                new Vector3Int(4, 2, 0),
                new Vector3Int(9, 9, 0),
                new Vector3Int(9, 7, 0),
                new Vector3Int(7, 9, 0),
                new Vector3Int(7, 7, 0),
                new Vector3Int(9, 4, 0),
                new Vector3Int(9, 2, 0),
                new Vector3Int(7, 4, 0),
                new Vector3Int(7, 2, 0)
            };
            foreach (var adjustedGridPos in spawnTerrainPositions)
            {
                GameObject terrain = Instantiate(darknessTerrainPrefab);
                PlaceTerrainOnTile(terrain, adjustedGridPos);
            }
            List<Vector3Int> spawnBrushPositions = new List<Vector3Int>
            {
                new Vector3Int(3, 8, 0),
                new Vector3Int(0, 5, 0),
                new Vector3Int(1, 6, 0),
                new Vector3Int(5, 11, 0),
                new Vector3Int(6, 10, 0),
                new Vector3Int(11, 6, 0),
                new Vector3Int(10, 5, 0),
                new Vector3Int(5, 1, 0),
                new Vector3Int(6, 0, 0)

            };
            foreach (var adjustedGridPos in spawnBrushPositions)
            {
                GameObject brush = Instantiate(darknessBrushPrefab);
                PlaceBrushOnTile(brush, adjustedGridPos);
            }
        }
    }
    public int GetManhattan(Vector3Int tilePos1, Vector3Int tilePos2){
        return Mathf.Abs(tilePos1.x - tilePos2.x) + Mathf.Abs(tilePos1.y - tilePos2.y);
    }

    // a combat action is pending 
    public bool IsInTargetingMode()
    {
        return unitWaitingToAct != null && pendingAction != null;
    }

    // a healing or protect action is pending
    public bool IsInTargetingAllyMode(){
        return unitWaitingToAct != null && pendingAction != null  && (pendingAction[0] == 2 || pendingAction[0] ==3);
    }

}
