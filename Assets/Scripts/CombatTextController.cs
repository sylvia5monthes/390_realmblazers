using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// this class handles all the combat text that is displayed on the screen
// show damage, healing, and other combat-related messages for both the actor and receiver
public class CombatTextController : MonoBehaviour
{
    public GameObject combatTextPanel;
    public TMP_Text actorText;
    public TMP_Text receiverText;
    private string displayString1 = " dealt ";
    private string displayString2 = " damage to ";
    private string healString1 = " healed ";
    private string healString2 = " to ";
    private string healString3 = " health!";

    // Start is called before the first frame update
    void Start()
    {
        combatTextPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowHealAmount(float health, string actorName, string receiverName){
        StopAllCoroutines();
        StartCoroutine(DisplayHealerText(health, actorName, receiverName));
    }
    public void ShowProtect(string actorName, string receiverName){
        StopAllCoroutines();
        StartCoroutine(DisplayProtectText(actorName, receiverName));
    }

    public void ShowActorDamage(float damage, string actorName, string receiverName)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayActorText(damage, actorName, receiverName));
    }
    public void ShowReceiverDamage(float damage, string receiverName, string actorName)
    {
        StartCoroutine(DisplayReceiverText(damage, receiverName, actorName));
    }
    public void ShowDeathText(string unitName)
    {
        StopAllCoroutines();
        combatTextPanel.SetActive(true);
        actorText.text = $"{unitName} has fallen!";
        actorText.gameObject.SetActive(true);
        receiverText.gameObject.SetActive(false);

        // Auto-hide after 2 seconds
        StartCoroutine(HideAfterDelay());
    }
    private IEnumerator DisplayHealerText(float health, string actorName, string receiverName){
        combatTextPanel.SetActive(false);
        actorText.text = actorName + healString1 + receiverName + healString2 + health.ToString() + healString3;
        combatTextPanel.SetActive(true);
        actorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        actorText.gameObject.SetActive(false);
        combatTextPanel.SetActive(false);
    }
    private IEnumerator DisplayProtectText(string actorName, string receiverName){
        combatTextPanel.SetActive(false);
        actorText.text = actorName + " protected " + receiverName + " for 3 turns!";
        combatTextPanel.SetActive(true);
        actorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        actorText.gameObject.SetActive(false);
        combatTextPanel.SetActive(false);
    }

    private IEnumerator DisplayActorText(float damage, string actorName, string receiverName){
        combatTextPanel.SetActive(false);
        actorText.text = actorName + displayString1 + damage.ToString() + displayString2 + receiverName + "!";
        combatTextPanel.SetActive(true);
        actorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        actorText.gameObject.SetActive(false);
    }
    private IEnumerator DisplayReceiverText(float damage, string receiverName, string actorName){
        receiverText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        if (damage > 0){
            receiverText.text = receiverName + displayString1 + damage.ToString() + displayString2 + actorName + "!";
            receiverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3.0f);
            receiverText.gameObject.SetActive(false);
            combatTextPanel.SetActive(false);
        }
        else{
            combatTextPanel.SetActive(false);
        }
    }
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(3.0f);
        actorText.gameObject.SetActive(false);
        combatTextPanel.SetActive(false);
    }

    public void HideAllCombatText()
    {
        StopAllCoroutines();
        actorText.gameObject.SetActive(false);
        receiverText.gameObject.SetActive(false);
        combatTextPanel.SetActive(false);
    }
}
