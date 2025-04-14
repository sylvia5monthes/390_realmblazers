using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CombatManager : MonoBehaviour
{
    //we can probably handle calling of animations using this script
    public static CombatManager Instance;
    private CombatTextController combatTextController;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        combatTextController = FindObjectOfType<CombatTextController>();
    }
    public void HandleCombat(Unit actor, Unit receiver, float[] action, int distance){
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
        float totaldamage = action[1] + offense - defense;
        float chance = action[2] * actor.prec / receiver.eva;
        if (Random.value>=chance){
            totaldamage = 0;//set damage to 0 if miss
        }
        receiver.currentHealth -=totaldamage;
        combatTextController.ShowActorDamage(totaldamage, actor.unitDisplayName, receiver.unitDisplayName);
        //RECEIVER'S ACTION: SKIP IF RECEIVER DIED
        if (receiver.currentHealth > 0){
            float[] defaultAction = receiver.actions[0];
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
                chance = defaultAction[2] * receiver.prec / actor.eva;
                if (Random.value>=chance){
                    totaldamage = 0;//set damage to 0 if miss
                }
                actor.currentHealth -=totaldamage;
            } else{
                totaldamage = 0;
            }
        } else{
            totaldamage=0;
        }
        combatTextController.ShowReceiverDamage(totaldamage, receiver.unitDisplayName,actor.unitDisplayName);
    }
}