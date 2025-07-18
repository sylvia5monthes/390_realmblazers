using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Unity.Mathematics;

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
    public Button vanguardButton;
    public GameObject abilityPanel;
    public Button abilityButtonTemplate;
    private bool knightPlaced = false;
    private bool archerPlaced = false;
    private bool whiteMagePlaced = false;
    private bool blackMagePlaced = false;
    private bool vanguardPlaced = false;

    public GameObject knightPrefab; 
    public GameObject archerPrefab;
    public GameObject whiteMagePrefab;
    public GameObject blackMagePrefab;
    public GameObject vanguardPrefab;

    [Header("Unit Action Menu")]
    public GameObject unitActionPanel;
    public Button moveButton;
    public Button actionButton;
    public Button waitButton;
    public Button closeActionMenuButton;
    public TMP_Text statsText;
    public TMP_Text unitNameText;
    public TMP_Text enemyAbilityText;
    private bool suppressClickThisFrame = false;


    [Header("Ability Tooltip")]
    public GameObject abilityTooltipPanel;
    public TMP_Text abilityTooltipText;

    private GameObject selectedUnitPrefab;
    private Unit selectedUnit;
    private GridManager gridManager;
    private List<Unit> activeUnits;
    public int level;
    public GameObject magmaDamageDisplay;
    private DialogueManager dialogueManager;
    public Sequence dialogue;
    private bool lastUnitUsedAction = false;
    string shieldBashDescription = "Reduces target's defenses for three turns.";
    string cureDescription = "Heals ally HP for user's magic attack.";
    string protectDescription = "Increases ally's defenses for three turns.";

    // Start is called before the first frame update
    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        // set button listeners to select unit type
        knightButton.onClick.AddListener(() => SelectUnitForPlacement(knightPrefab));
        archerButton.onClick.AddListener(() => SelectUnitForPlacement(archerPrefab));
        whiteMageButton.onClick.AddListener(() => SelectUnitForPlacement(whiteMagePrefab));
        if (level >= 2){
            blackMageButton.onClick.AddListener(() => SelectUnitForPlacement(blackMagePrefab));
        }
        if (level == 3){
            vanguardButton.onClick.AddListener(() => SelectUnitForPlacement(vanguardPrefab));
        }

        // set button listeners to open menus
        minimizeButton.onClick.AddListener(HideUnitSelectionMenu);
        openUnitSelectionButton.onClick.AddListener(ShowUnitSelectionMenu);
        openUnitSelectionButton.gameObject.SetActive(false);

        // set button listeners for action panel
        moveButton.onClick.AddListener(() =>
        {
            if (suppressClickThisFrame) return;
            MoveUnit();
        });

        actionButton.onClick.AddListener(() =>
        {
            if (suppressClickThisFrame) return;
            OnActionButtonPressed();
        });

        waitButton.onClick.AddListener(() =>
        {
            if (suppressClickThisFrame) return;
            WaitUnit();
        });

        closeActionMenuButton.onClick.AddListener(() =>
        {
            if (suppressClickThisFrame) return;
            HideUnitActionMenu();
        });
        
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

    // mark the unit as placed and disable the button
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
        } else if (prefab == vanguardPrefab){
            vanguardPlaced = true;
            vanguardButton.interactable = false;
            activeUnits.Add(FindObjectOfType<VanguardUnit>());
        }
        // check if all units are placed, and if so start the dialogue
        if (level == 2){
            if (knightPlaced && archerPlaced && whiteMagePlaced && blackMagePlaced) 
            {
            HideUnitSelectionMenu();

            // clear highlighted initial placement tiles
            FindObjectOfType<GridManager>().ClearInitialPlacementHighlightedTiles();

            openUnitSelectionButton.gameObject.SetActive(false);
            Debug.Log("All units placed. Starting dialogue.");

            dialogueManager = FindObjectOfType<DialogueManager>();

            dialogueManager.StartDialogue(dialogue);
            }
        } else if (level == 3){
            if (knightPlaced && archerPlaced && whiteMagePlaced && blackMagePlaced && vanguardPlaced) 
            {
            HideUnitSelectionMenu();

            // clear highlighted initial placement tiles
            FindObjectOfType<GridManager>().ClearInitialPlacementHighlightedTiles();

            openUnitSelectionButton.gameObject.SetActive(false);
            Debug.Log("All units placed. Starting dialogue.");

            dialogueManager = FindObjectOfType<DialogueManager>();

            dialogueManager.StartDialogue(dialogue);
            }
            } 
            else
            {
                if (knightPlaced && archerPlaced && whiteMagePlaced) 
                {
                HideUnitSelectionMenu();
                
                // clear highlighted initial placement tiles
                FindObjectOfType<GridManager>().ClearInitialPlacementHighlightedTiles();

                openUnitSelectionButton.gameObject.SetActive(false);
                Debug.Log("All units placed. Starting dialogue.");

                dialogueManager = FindObjectOfType<DialogueManager>();

                dialogueManager.StartDialogue(dialogue);
                }
        }
    }

    // shows the unit action menu for the selected unit
    public void ShowUnitActionMenu(Unit unit)
    {
        Debug.Log($"Showing action menu for unit: {unit.name}");
        selectedUnit = unit;
        unitActionPanel.SetActive(true);

        moveButton.interactable = !(unit.hasMoved || unit.hasActed);
        actionButton.interactable = !unit.hasActed;
        waitButton.interactable = !unit.hasActed;

        // enemy unit: show ability info and hide action buttons
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
                string actionType = "Physical";
                string description = "";
                if (stats[0] == 1)
                {
                    actionType = "Magical";
                }
                else if (stats[0] > 1)
                {
                    actionType = "Support";
                }
                if (actionName == "Shield Bash")
                {
                    description = shieldBashDescription;
                }
                else if (actionName == "Cure")
                {
                    description = cureDescription;
                }
                else if (actionName == "Protect")
                {
                    description = protectDescription;
                }

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
                        $"Type: {actionType}\nPower: {stats[1]}\nAccuracy: {stats[2]}\nRange: {stats[3]}\n{description}";

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
        float unitDisplayEva = unit.eva;
        if(gridManager.TileAt(unit.currentTilePos).isBrush){
            unitDisplayEva = Mathf.Floor(unit.eva * 1.2f);
        }
        float unitDisplayDef = unit.def;
        float unitDisplayMdef = unit.mdef;
        // handle buffs and debuffs
        if (unit.defenseBuffed){
            unitDisplayDef = Mathf.Floor(unit.def * 1.2f);
            unitDisplayMdef = Mathf.Floor(unit.mdef * 1.2f);
        } else if (unit.defenseDebuffed){
            unitDisplayDef = Mathf.Floor(unit.def * 0.8f);
            unitDisplayMdef = Mathf.Floor(unit.mdef * 0.8f);
        }
        statsText.text = $"Health: {unit.currentHealth} / {unit.health}\n" +
                        $"Attack: {unit.atk}\n" +
                        $"Defense: {unitDisplayDef}\n" +
                        $"Magic Attack: {unit.matk}\n" +
                        $"Magic Defense: {unitDisplayMdef}\n" +
                        $"Precision: {unit.prec}\n" +
                        $"Evasion: {unitDisplayEva}\n" +
                        $"Movement: {unit.mov}";
        
        suppressClickThisFrame = true;
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
        lastUnitUsedAction = false;
        UnitHasActed(selectedUnit);//button
    }

    public void UnitHasActed(Unit unit){
        HideUnitActionMenu();
        unit.hasActed = true;
        if (!unit.hasMoved){//waiting ends the unit's turn. if anyone disagrees we can change, but right now
            unit.hasMoved = true; //i think it's easiest to just whenever we have a unit act we automatically set moved to true as well
        }
        gridManager.ClearAction();
        if (gridManager.TileAt(unit.currentTilePos).isMagma && unit.currentHealth > 0)
        {
            Debug.Log("Magma damage");
            Tilemap tilemap = FindObjectOfType<Tilemap>();
            unit.currentHealth -= 3;
            Vector3 displayPos = tilemap.GetCellCenterWorld(gridManager.GridPositionToWorldPosition(unit.currentTilePos));
            Instantiate(magmaDamageDisplay, displayPos, Quaternion.identity);
            if (lastUnitUsedAction)
            {
                unit.CheckDelayDeath();
            }
            else
            {
                unit.CheckDeath();
            }
        }
        if (AllPlayersActed())
        {
            Debug.Log("all acted");
            // delay enemy phase start if last unit used an action because there are combat popups showing up
            if (lastUnitUsedAction)
            {
                StartCoroutine(DelayedStartEnemyPhase(5.0f));
            }
            else
            {
                FindObjectOfType<GameManager>()?.StartEnemyPhase();
            }
            return;
        }
    }

    private IEnumerator DelayedStartEnemyPhase(float delay)
    {
        yield return new WaitForSeconds(delay);
        FindObjectOfType<GameManager>()?.StartEnemyPhase();
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
        lastUnitUsedAction = true;
        Debug.Log($"Action selected: {selectedUnit.actionNames[actionIndex]}");
        //selectedUnit.PerformAction(actionIndex);
        HideUnitActionMenu();
        gridManager.StartAction(selectedUnit, selectedUnit.actions[actionIndex], selectedUnit.actionNames[actionIndex]);//4th value in the array is range
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
        if (suppressClickThisFrame && Input.GetMouseButtonUp(0))
        {
            suppressClickThisFrame = false;
        }
    }
    public void PhaseStart(){//re-enable actions and movements on all active units at start of phase
        //Tilemap tilemap = FindObjectOfType<Tilemap>();
        foreach (Unit unit in activeUnits)
        {
            unit.hasActed = false;
            unit.hasMoved = false;
            /*if (gridManager.TileAt(unit.currentTilePos).isMagma){
                    unit.currentHealth-=3;
                    Vector3 displayPos = tilemap.GetCellCenterWorld(gridManager.GridPositionToWorldPosition(unit.currentTilePos));
                    Instantiate(magmaDamageDisplay, displayPos, Quaternion.identity);
                }*/
            unit.DecrementBuff();
            
        }

    }
    public List<Unit> GetUnits(){
        return activeUnits;
    }
}
