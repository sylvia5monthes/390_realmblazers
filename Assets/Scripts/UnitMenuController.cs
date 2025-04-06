using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitMenuController : MonoBehaviour
{
    public GameObject menuPanel;
    public Button moveButton;
    public Button actionButton;

    private Unit selectedUnit;
    private GridManager gridManager;
    // Start is called before the first frame update
    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        menuPanel.SetActive(false);
    }

    public void ShowMenu(Unit unit)
    {
        selectedUnit = unit;

        moveButton.interactable = !unit.hasMoved;
        actionButton.interactable = !unit.hasAttacked;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
        menuPanel.transform.position = screenPos;

        menuPanel.SetActive(true);
    }

    public void HideMenu()
    {
        menuPanel.SetActive(false);
        selectedUnit = null;
    }

    public void OnMoveClicked()
    {
        if (selectedUnit != null)
        {
            Vector3Int adjustedGridPos = gridManager.WorldPositionToGridPosition(selectedUnit.currentTilePos);
            gridManager.HighlightMovableTiles(selectedUnit.currentTilePos);
        }

        HideMenu(); 
    }

    public void OnAttackClicked()
    {
        if (selectedUnit != null)
        {
            selectedUnit.hasAttacked = true;
        }

        HideMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
