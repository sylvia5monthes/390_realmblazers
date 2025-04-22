using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueUI;
    public TMP_Text speakerText;
    public TMP_Text dialogueText;
    public Image portraitImage;

    private Queue<Line> lines;
    private bool isDialoguePlaying;

    void Start()
    {
        dialogueUI.SetActive(false);
    }

    public void StartDialogue(Sequence sequence)
    {
        lines = new Queue<Line>(sequence.lines);
        isDialoguePlaying = true;
        dialogueUI.SetActive(true);
        DisplayNextLine();
    }

    void Update()
    {
        if (isDialoguePlaying && Input.GetMouseButtonDown(0)) // Left click
        {
            DisplayNextLine();
        }
    }

    void DisplayNextLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        Line line = lines.Dequeue();
        speakerText.text = line.speaker;
        dialogueText.text = line.text;
        portraitImage.sprite = line.portrait;
    }

    void EndDialogue()
    {
        isDialoguePlaying = false;
        dialogueUI.SetActive(false);

        FindObjectOfType<GameManager>()?.StartPlayerPhase();
    }

    public bool IsDialoguePlaying()
    {
        return isDialoguePlaying;
    }
}

