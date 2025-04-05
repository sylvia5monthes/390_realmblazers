using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap highlighter;
    public TileBase highlightedTile;
    public int width = 10;
    public int height = 10;

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
        }
    }

    bool IsInBounds(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    void HighlightTileAt(Vector3Int gridPos)
    {
        highlighter.ClearAllTiles(); // Remove old highlights
        highlighter.SetTile(gridPos, highlightedTile);
    }

}
