using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CombatTextController : MonoBehaviour
{
    public GameObject combatTextPanel;
    public TMP_Text actorText;
    public TMP_Text defenderText;

    // Start is called before the first frame update
    void Start()
    {
        combatTextPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowActorDamage(float damage)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayActorText(damage));
    }

    private IEnumerator DisplayActorText(float damage){
        yield return new WaitForSeconds(3f);
    }

    /*private IEnumerator DisplayActorText(string phase)
    {

        actorText.text = phase;

        phaseText.gameObject.SetActive(true);
        phaseTextPanel.SetActive(true);

        yield return new WaitForSeconds(3f);

        phaseText.gameObject.SetActive(false);
        phaseTextPanel.SetActive(false);
        GameManager.Instance.isPhaseChanging = false;
    }
    private IEnumerator DisplayPhaseText(string phase)
    {

        phaseText.text = phase;

        phaseText.gameObject.SetActive(true);
        phaseTextPanel.SetActive(true);

        yield return new WaitForSeconds(3f);

        phaseText.gameObject.SetActive(false);
        phaseTextPanel.SetActive(false);
        GameManager.Instance.isPhaseChanging = false;
    }*/
}
