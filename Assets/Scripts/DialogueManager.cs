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
    private Queue<Line> endLines;
    private bool isDialoguePlaying;
    private bool isEndDialoguePlaying;

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

    public void StartEndDialogue(Sequence endSequence)
    {
        endLines = new Queue<Line>(endSequence.lines);
        isEndDialoguePlaying = true;
        dialogueUI.SetActive(true);
        DisplayNextEndLine();
    }

    void Update()
    {
        if (isDialoguePlaying && Input.GetMouseButtonDown(0)) // Left click
        {
            DisplayNextLine();
        }
        if (isEndDialoguePlaying && Input.GetMouseButtonDown(0))
        {
            DisplayNextEndLine();
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

    void DisplayNextEndLine()
    {
        if (endLines.Count == 0)
        {
            EndEndDialogue();
            return;
        }

        Line line = endLines.Dequeue();
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

    void EndEndDialogue()
    {
        isEndDialoguePlaying = false;
        dialogueUI.SetActive(false);

        FindObjectOfType<GameManager>()?.LoadNext();
    }

    public bool IsDialoguePlaying()
    {
        return isDialoguePlaying;
    }
}

