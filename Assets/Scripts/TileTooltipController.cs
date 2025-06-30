using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TileTooltipController : MonoBehaviour
{
    public Button closeTooltipButton;
    public Button showTooltipButton;
    public TMP_Text tileType;
    public TMP_Text tileDescription;
    public GameObject tileTooltip;

    public void ShowTileTooltip()
    {
        tileTooltip.SetActive(true);
        showTooltipButton.gameObject.SetActive(false);
    }
    public void HideTileTooltip()
    {
        tileTooltip.SetActive(false);
        showTooltipButton.gameObject.SetActive(true);
    }

    public void ChangeDescription(Tile selectedTile)
    {
        if (selectedTile.isBrush)
        {
            tileType.text = "Brush";
            tileDescription.text = "Increases unit's evasion.";
        }
        else if (selectedTile.isMagma)
        {
            tileType.text = "Magma";
            tileDescription.text = "Damages unit by 3 after unit's action.";
        }
        else if (selectedTile.isTerrain)
        {
            tileType.text = "Terrain";
            tileDescription.text = "Impassable by units.";
        }
        else
        {
            tileType.text = "";
            tileDescription.text = "No additional effect.";
        }
    }

    
}
