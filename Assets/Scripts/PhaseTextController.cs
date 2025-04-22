using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhaseTextController : MonoBehaviour
{
    public GameObject phaseTextPanel;
    public TMP_Text phaseText;


    // Start is called before the first frame update
    void Start()
    {
        phaseTextPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPhase(string phase)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayPhaseText(phase));
    }

    // display the phase text for a short duration
    private IEnumerator DisplayPhaseText(string phase)
    {

        phaseText.text = phase;

        phaseText.gameObject.SetActive(true);
        phaseTextPanel.SetActive(true);

        yield return new WaitForSeconds(3f);

        phaseText.gameObject.SetActive(false);
        phaseTextPanel.SetActive(false);
        GameManager.Instance.isPhaseChanging = false;
    }
}
