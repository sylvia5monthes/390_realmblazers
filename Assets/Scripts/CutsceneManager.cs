using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    
    [System.Serializable]
    public class CutsceneSlide
    {
        public Sprite image;
        [TextArea] public string message;
    }

    public GameObject continuePrompt;

    public CutsceneSlide[] slides;
    public Image cutsceneImage;
    public CanvasGroup cutsceneCanvas;
    public TextMeshProUGUI cutsceneText;
    public float fade = 0.5f;
    public float typeSpeed = 0.05f;

    private int currentSlideIndex = 0;
    private bool isTyping = false;
    private bool readyForNext = false;


    void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && readyForNext && !isTyping)
        {
            currentSlideIndex++;
            if (currentSlideIndex >= slides.Length)
            {
                SceneManager.LoadScene("SampleScene");
            }
            else 
            {
                StartCoroutine(ShowSlide(currentSlideIndex));
            }
        }
    }

    IEnumerator PlayCutscene()
    {
        cutsceneCanvas.alpha = 0f;
        yield return StartCoroutine(ShowSlide(0));
    }

    IEnumerator FadeIn(float targetAlpha, float duration)
    {
        float startAlpha = cutsceneCanvas.alpha;
        float elapsedTime = 0f;
        while (elapsedTime < fade)
        {
            cutsceneCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cutsceneCanvas.alpha = targetAlpha;
    }

    IEnumerator ShowSlide(int index)
    {
        continuePrompt.SetActive(false);
        readyForNext = false;
        cutsceneCanvas.alpha = 0f;

        cutsceneImage.sprite = slides[index].image;
        cutsceneText.text = "";

        yield return StartCoroutine(FadeIn(1f, fade));

        string message = slides[index].message;
        isTyping = true;
        

        foreach (char letter in message.ToCharArray())
        {
            cutsceneText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        readyForNext = true;

        while (readyForNext)
        {
            continuePrompt.SetActive(!continuePrompt.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
        continuePrompt.SetActive(false);
    }
}
