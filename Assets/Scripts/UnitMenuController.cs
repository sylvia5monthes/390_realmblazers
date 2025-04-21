using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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
    public Button blackMageButton;
    public GameObject abilityPanel;
    public Button abilityButtonTemplate;
    private bool knightPlaced = false;
    private bool archerPlaced = false;
    private bool whiteMagePlaced = false;
    private bool blackMagePlaced = false;

    public GameObject knightPrefab; 
    public GameObject archerPrefab;
    public GameObject whiteMagePrefab;
    public GameObject blackMagePrefab;

    [Header("Unit Action Menu")]
    public GameObject unitActionPanel;
    public Button moveButton;
    public Button actionButton;
    public Button waitButton;
    public TMP_Text statsText;
    public TMP_Text unitNameText;
    public TMP_Text enemyAbilityText;


    [Header("Ability Tooltip")]
    public GameObject abilityTooltipPanel;
    public TMP_Text abilityTooltipText;

    private GameObject selectedUnitPrefab;
    private Unit selectedUnit;
    private GridManager gridManager;
    private List<Unit> activeUnits;
    public int level;

    // Start is called before the first frame update
    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        // set button listeners to select unit type
        knightButton.onClick.AddListener(() => SelectUnitForPlacement(knightPrefab));
        archerButton.onClick.AddListener(() => SelectUnitForPlacement(archerPrefab));
        whiteMageButton.onClick.AddListener(() => SelectUnitForPlacement(whiteMagePrefab));
        if (level == 2){
            blackMageButton.onClick.AddListener(() => SelectUnitForPlacement(blackMagePrefab));
        }

        // set button listeners to open menus
        minimizeButton.onClick.AddListener(HideUnitSelectionMenu);
        openUnitSelectionButton.onClick.AddListener(ShowUnitSelectionMenu);
        openUnitSelectionButton.gameObject.SetActive(false);
        
        unitSelectionPanel.SetActive(true);
        unitActionPanel.SetActive(false);
        abilityPanel.SetActive(false);
        abilityTooltipPanel.SetActive(false);
        activeUnits = new List<Unit>();

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
            activeUnits.Add(FindObjectOfType<KnightUnit>());
        } 
        else if (prefab == archerPrefab) 
        {
            archerPlaced = true;
            archerButton.interactable = false;
            activeUnits.Add(FindObjectOfType<ArcherUnit>());
        } 
        else if (prefab == whiteMagePrefab) 
        {
            whiteMagePlaced = true;
            whiteMageButton.interactable = false;
            activeUnits.Add(FindObjectOfType<WhiteMageUnit>());
        } else if (prefab == blackMagePrefab){
            blackMagePlaced = true;
            blackMageButton.interactable = false;
            activeUnits.Add(FindObjectOfType<BlackMageUnit>());
        }
        if (level == 2){
            if (knightPlaced && archerPlaced && whiteMagePlaced && blackMagePlaced) 
            {
            HideUnitSelectionMenu();
            openUnitSelectionButton.gameObject.SetActive(false);
            Debug.Log("All units placed. Starting player phase.");

            FindObjectOfType<GameManager>()?.StartPlayerPhase();
            }
        } else{
            if (knightPlaced && archerPlaced && whiteMagePlaced) 
            {
            HideUnitSelectionMenu();
            openUnitSelectionButton.gameObject.SetActive(false);
            Debug.Log("All units placed. Starting player phase.");

            FindObjectOfType<GameManager>()?.StartPlayerPhase();
            }
        }
    }

    public void ShowUnitActionMenu(Unit unit)
    {
        Debug.Log($"Showing action menu for unit: {unit.name}");
        selectedUnit = unit;
        unitActionPanel.SetActive(true);

        moveButton.interactable = !(unit.hasMoved || unit.hasActed);
        actionButton.interactable = !unit.hasActed;
        waitButton.interactable = !unit.hasActed;

        if (unit.isEnemy)
        {
            // Hide action buttons
            moveButton.gameObject.SetActive(false);
            actionButton.gameObject.SetActive(false);
            waitButton.gameObject.SetActive(false);
            enemyAbilityText.gameObject.SetActive(true);

            string abilityInfo = "";
            for (int i = 0; i < unit.actionNames.Length; i++)
            {
                string actionName = unit.actionNames[i];
                float[] stats = unit.actions[i];

                abilityInfo += $"{actionName}\n";
                abilityInfo += $"  Type: {(stats[0] == 0 ? "Physical" : "Magic")}\n";
                abilityInfo += $"  Power: {stats[1]}\n";
                abilityInfo += $"  Accuracy: {stats[2]}\n";
                abilityInfo += $"  Range: {stats[3]}\n\n";

                Debug.Log($"[ENEMY] {actionName}: P={stats[1]}, Acc={stats[2]}, R={stats[3]}");
            }
            enemyAbilityText.text = abilityInfo;
        }
        else
        {
            // Player unit: show action buttons and interactive tooltips
            moveButton.gameObject.SetActive(true);
            actionButton.gameObject.SetActive(true);
            waitButton.gameObject.SetActive(true);
            enemyAbilityText.gameObject.SetActive(false);


            abilityPanel.SetActive(true);
            foreach (Transform child in abilityPanel.transform)
            {
                if (child != abilityButtonTemplate.transform)
                    Destroy(child.gameObject);
            }

            for (int i = 0; i < unit.actionNames.Length; i++)
            {
                string actionName = unit.actionNames[i];
                float[] stats = unit.actions[i];
                int actionIndex = i;

                Debug.Log($"[PLAYER ABILITY] {actionName}");

                Button newButton = Instantiate(abilityButtonTemplate, abilityPanel.transform);
                newButton.gameObject.SetActive(true);
                newButton.interactable = !unit.hasActed;
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = actionName;

                newButton.onClick.AddListener(() => DoAction(actionIndex));

                // Tooltip hover setup (unchanged)
                EventTrigger trigger = newButton.gameObject.AddComponent<EventTrigger>();

                EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
                enter.callback.AddListener((data) => {
                    abilityTooltipPanel.SetActive(true);
                    abilityTooltipText.text = 
                        $"{actionName}\nPower: {stats[1]}\nAccuracy: {stats[2]}\nRange: {stats[3]}";

                    Vector2 anchoredPos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        unitActionPanel.GetComponent<RectTransform>(),
                        Input.mousePosition,
                        null,
                        out anchoredPos
                    );
                    Vector2 tooltipOffset = new Vector2(10, -20);
                    abilityTooltipPanel.GetComponent<RectTransform>().anchoredPosition = anchoredPos + tooltipOffset;
                });

                EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
                exit.callback.AddListener((data) => {
                    abilityTooltipPanel.SetActive(false);
                });

                trigger.triggers.Add(enter);
                trigger.triggers.Add(exit);
            }
        }
        unitNameText.text = unit.unitDisplayName;
        statsText.text = $"Health: {unit.currentHealth} / {unit.health}\n" +
                        $"Attack: {unit.atk}\n" +
                        $"Defense: {unit.def}\n" +
                        $"Magic Attack: {unit.matk}\n" +
                        $"Magic Defense: {unit.mdef}\n" +
                        $"Precision: {unit.prec}\n" +
                        $"Evasion: {unit.eva}\n" +
                        $"Movement: {unit.mov}";
        
    }

    public void HideUnitActionMenu()
    {
        unitActionPanel.SetActive(false);
        abilityPanel.SetActive(false);
        abilityTooltipPanel.SetActive(false);
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

    public bool AllPlayersActed()
    {
        foreach (Unit unit in activeUnits)
        {
            if (!unit.hasActed){
                return false;
            }
        }
        return true;
    }

    public void MoveUnit(){
        gridManager.StartMove(selectedUnit);//button
        HideUnitActionMenu();
    }
    public void UnitHasMoved(Unit unit){//call after each Unit Action
        unit.hasMoved = true;
        ShowUnitActionMenu(unit);
    }
    public void WaitUnit(){
        UnitHasActed(selectedUnit);//button
    }

    public void UnitHasActed(Unit unit){
        HideUnitActionMenu();
        unit.hasActed = true;
        if (!unit.hasMoved){//waiting ends the unit's turn. if anyone disagrees we can change, but right now
            unit.hasMoved = true; //i think it's easiest to just whenever we have a unit act we automatically set moved to true as well
        }
        if (AllPlayersActed()){
            FindObjectOfType<GameManager>()?.StartEnemyPhase();
            return;
        }
    }
    public void OnActionButtonPressed()//ability menu
    {
        // // Clear old buttons
        // foreach (Transform child in abilityPanel.transform)
        // {
        //     if (child != abilityButtonTemplate.transform)
        //         Destroy(child.gameObject);
        // }

        // // Show panel
        // abilityPanel.SetActive(!abilityPanel.activeSelf);

        // // Create new buttons based on actionNames
        // for (int i = 0; i < selectedUnit.actionNames.Length; i++)
        // {
        //     string actionName = selectedUnit.actionNames[i];
        //     int actionIndex = i; // Capture for lambda

        //     Button newButton = Instantiate(abilityButtonTemplate, abilityPanel.transform);
        //     newButton.gameObject.SetActive(true);
        //     newButton.GetComponentInChildren<TextMeshProUGUI>().text = actionName;

        //     newButton.onClick.AddListener(() =>
        //     {
        //         DoAction(actionIndex);
        //     });
        // }
    }//things to consider- self targeted actions?healing?right now this is just for combat abilities, but will try to restructure later
    private void DoAction(int actionIndex)
    {
        Debug.Log($"Action selected: {selectedUnit.actionNames[actionIndex]}");
        //selectedUnit.PerformAction(actionIndex);
        HideUnitActionMenu();
        gridManager.StartAction(selectedUnit, selectedUnit.actions[actionIndex]);//4th value in the array is range
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
    public void PhaseStart(){//re-enable actions and movements on all active units at start of phase
        foreach (Unit unit in activeUnits)
        {
            unit.hasActed = false;
            unit.hasMoved = false;
        }
    }
    public List<Unit> GetUnits(){
        return activeUnits;
    }
}
