using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitMenuController : MonoBehaviour
{
    // buttons for opening menus 
    public Button minimizeButton;
    public Button openUnitSelectionButton;
    
    // initial options for where to place units
    // TODO: make a list of buttons for available units
    [Header("Unit Selection Menu")]
    public GameObject unitSelectionPanel;
    public Button knightButton;
    public Button archerButton;
    public Button whiteMageButton;
    private bool knightPlaced = false;
    private bool archerPlaced = false;
    private bool whiteMagePlaced = false;

    public GameObject knightPrefab; 
    public GameObject archerPrefab;
    public GameObject whiteMagePrefab;

    [Header("Unit Action Menu")]
    public GameObject unitActionPanel;
    public Button moveButton;
    public Button actionButton;

    private GameObject selectedUnitPrefab;
    private Unit selectedUnit;
    private GridManager gridManager;

    // Start is called before the first frame update
    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        // set button listeners to select unit type
        knightButton.onClick.AddListener(() => SelectUnitForPlacement(knightPrefab));
        archerButton.onClick.AddListener(() => SelectUnitForPlacement(archerPrefab));
        whiteMageButton.onClick.AddListener(() => SelectUnitForPlacement(whiteMagePrefab));

        // set button listeners to open menus
        minimizeButton.onClick.AddListener(HideUnitSelectionMenu);
        openUnitSelectionButton.onClick.AddListener(ShowUnitSelectionMenu);
        openUnitSelectionButton.gameObject.SetActive(false);
        
        unitSelectionPanel.SetActive(true);
        unitActionPanel.SetActive(false);

    }

    // set the selected unit prefab
    public void SelectUnitForPlacement(GameObject unitPrefab) 
    {
        selectedUnitPrefab = unitPrefab;
        gridManager.SetSelectedUnitPrefab(unitPrefab);
        Debug.Log($"Selected unit prefab: {selectedUnitPrefab.name}");
    }

    public void MarkUnitAsPlaced(GameObject prefab) 
    {
        if (prefab == knightPrefab) 
        {
            knightPlaced = true;
            knightButton.interactable = false;
        } 
        else if (prefab == archerPrefab) 
        {
            archerPlaced = true;
            archerButton.interactable = false;
        } 
        else if (prefab == whiteMagePrefab) 
        {
            whiteMagePlaced = true;
            whiteMageButton.interactable = false;
        }

        if (knightPlaced && archerPlaced && whiteMagePlaced) 
        {
            HideUnitSelectionMenu();
            openUnitSelectionButton.gameObject.SetActive(false);
            Debug.Log("All units placed. Hiding unit selection menu.");
        }
    }

    public void ShowUnitActionMenu(Unit unit)
    {
        selectedUnit = unit;
        unitActionPanel.SetActive(true);
        moveButton.interactable = !unit.hasMoved;
        actionButton.interactable = !unit.hasAttacked;
        // TODO: add stats text
    }

    public void HideUnitActionMenu()
    {
        unitActionPanel.SetActive(false);
        //selectedUnit = null;
    }

    public void ShowUnitSelectionMenu()
    {
        unitSelectionPanel.SetActive(true);
        openUnitSelectionButton.gameObject.SetActive(false);
    }

    public void HideUnitSelectionMenu()
    {
        unitSelectionPanel.SetActive(false);
        openUnitSelectionButton.gameObject.SetActive(true);
    }

    // public void OnMoveClicked()
    // {
    //     if (selectedUnit != null)
    //     {
    //         Vector3Int adjustedGridPos = gridManager.WorldPositionToGridPosition(selectedUnit.currentTilePos);
    //         gridManager.HighlightMovableTiles(selectedUnit.currentTilePos);
    //         gridManager.StartMove(selectedUnit); 
    //     }
    //     HideMenu(); 
        
    // }

    // public void OnAttackClicked()
    // {
    //     if (selectedUnit != null)
    //     {
    //         selectedUnit.hasAttacked = true;
    //     }

    //     HideMenu();
    // }

    // Update is called once per frame
    void Update()
    {
        
    }
}
