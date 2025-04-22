using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CombatManager : MonoBehaviour
{
    //we can probably handle calling of animations using this script
    public static CombatManager Instance;
    private CombatTextController combatTextController;
    private GridManager gridmanager;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        combatTextController = FindObjectOfType<CombatTextController>();
        gridmanager = FindObjectOfType<GridManager>();
    }
    public void HandleHeal(Unit actor, Unit receiver, float[] action){
        float totalheal = action[1] + actor.matk;
        receiver.currentHealth = Mathf.Min(receiver.health, receiver.currentHealth+totalheal);
        combatTextController.ShowHealAmount(receiver.currentHealth, actor.unitDisplayName, receiver.unitDisplayName);

        // display combat animation
        GridManager gridManager = FindObjectOfType<GridManager>();
        Tilemap tilemap = FindObjectOfType<Tilemap>();
        Vector3 vfxPos = tilemap.GetCellCenterWorld(gridManager.GridPositionToWorldPosition(receiver.currentTilePos));
        VFXManager.Instance?.PlayActionVFX("Cure", vfxPos);
    }
    public void HandleCombat(Unit actor, Unit receiver, float[] action, string actionName, int distance) {
        Debug.Log("CombatManager: HandleCombat called between " + actor.unitDisplayName + " and " + receiver.unitDisplayName);
        bool magic = false;
        if (action[0] == 1){
            magic = true;
            Debug.Log("Magical damage!!!!!");
        }
        float offense;
        float defense;
        //ACTOR'S ACTION: SETTING STATS
        if(!magic){
            offense = actor.atk;
            defense = receiver.def;
        } else{
            offense = actor.matk;
            defense = receiver.mdef;
        }
        float chance;
        if (gridmanager.TileAt(receiver.currentTilePos).isBrush){
            chance = action[2] * actor.prec / (receiver.eva * 1.2f);
            Debug.Log(actor.eva/1.2f);
        } else{
            chance = action[2] * actor.prec / receiver.eva;
        }
        float totaldamage = action[1] + offense - defense;
        if (Random.value>=chance){
            totaldamage = 0;//set damage to 0 if miss
        }
        receiver.currentHealth -= totaldamage;
        combatTextController.ShowActorDamage(totaldamage, actor.unitDisplayName, receiver.unitDisplayName);

        // display combat animation
        GridManager gridManager = FindObjectOfType<GridManager>();
        Tilemap tilemap = FindObjectOfType<Tilemap>();
        Vector3 vfxPos = tilemap.GetCellCenterWorld(gridManager.GridPositionToWorldPosition(receiver.currentTilePos));
        VFXManager.Instance?.PlayActionVFX(actionName, vfxPos);

        // Check death AFTER showing damage
        if (receiver.currentHealth <= 0)
        {
            StartCoroutine(HandleDeathAfterDelay(receiver));
            return;
        }
        //RECEIVER'S ACTION: SKIP IF RECEIVER DIED
        if (receiver.currentHealth > 0){
            float[] defaultAction = receiver.actions[0];
            string defaultActionName = receiver.actionNames[0];
            if (distance <= defaultAction[3]){
                if (defaultAction[0] == 1){
                magic = true;
                } else{
                    magic = false;
                }
                if(!magic){
                    offense = receiver.atk;
                    defense = actor.def;
                } else{
                    offense = receiver.matk;
                    defense = actor.mdef;
                }
                totaldamage = defaultAction[1] + offense - defense;
                if (gridmanager.TileAt(actor.currentTilePos).isBrush){
                    chance = defaultAction[2] * receiver.prec / (actor.eva * 1.2f);
                } else{
                    chance = defaultAction[2] * receiver.prec / actor.eva;
                }
                if (Random.value>=chance){
                    totaldamage = 0;//set damage to 0 if miss
                }
                actor.currentHealth -=totaldamage;

                // display combat animation
                vfxPos = tilemap.GetCellCenterWorld(gridManager.GridPositionToWorldPosition(actor.currentTilePos));
                VFXManager.Instance?.PlayActionVFX(defaultActionName, vfxPos);

            } else{
                totaldamage = 0;
            }
        } else{
            totaldamage=0;
        }
        combatTextController.ShowReceiverDamage(totaldamage, receiver.unitDisplayName,actor.unitDisplayName);
        if (actor.currentHealth <= 0)
        {
            StartCoroutine(HandleDeathAfterDelay(actor));
            return;
        }
    }

    private IEnumerator HandleDeathAfterDelay(Unit unit)
    {
        // Wait 2 seconds to let the damage text display
        yield return new WaitForSeconds(2f);

        unit.CheckDeath();  // This will show death text and clean up
    }
}