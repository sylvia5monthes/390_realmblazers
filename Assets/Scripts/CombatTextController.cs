using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CombatTextController : MonoBehaviour
{
    public GameObject combatTextPanel;
    public TMP_Text actorText;
    public TMP_Text receiverText;
    private string displayString1 = " dealt ";
    private string displayString2 = " damage to ";

    // Start is called before the first frame update
    void Start()
    {
        combatTextPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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

    private IEnumerator DisplayActorText(float damage, string actorName, string receiverName){
        combatTextPanel.SetActive(false);
        actorText.text = actorName + displayString1 + damage.ToString() + displayString2 + receiverName + "!";
        combatTextPanel.SetActive(true);
        actorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        actorText.gameObject.SetActive(false);
    }
    private IEnumerator DisplayReceiverText(float damage, string receiverName, string actorName){
        receiverText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        if (damage > 0){
            receiverText.text = receiverName + displayString1 + damage.ToString() + displayString2 + actorName + "!";
            receiverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            receiverText.gameObject.SetActive(false);
            combatTextPanel.SetActive(false);
        }
        else{
            combatTextPanel.SetActive(false);
        }
    }
}
