using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static WaveManager;

public class DialogueController : MonoBehaviour
{

    private Queue<string> sentences;

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueSpeedScaleIndicator;

    public CanvasGroup dialogueCanvasGroup;


    [HideInInspector]
    public bool startedTyping = false;

    private bool textComplete = false;

    private Dialogue currentDial;

    public float autoContinueTime;

    public AudioClip dialogueBlipSFX;

    [SerializeField]
    public float letterDisplayDelay;
    private float letterDisplayDelayOffset = -0.025f;
    private float minimumValue = 0.05f;
    private float maxValue = 0.15f;

    public delegate void DialogueEnded();
    public static event DialogueEnded DialogueOver;

    public delegate void DialogueBegun();
    public static event DialogueBegun DialogueStarted;

    // Start is called before the first frame update
    void Awake()
    {
        sentences = new Queue<string>();
        dialogueCanvasGroup = GameObject.Find("Dialogue System Canvas").GetComponent<CanvasGroup>();
    }
    public void OnEnable()
    {
        WaveEndDisplay += StartDialogue;
    }

    public void OnDisable()
    {
        WaveEndDisplay -= StartDialogue;
    }

    public void StartDialogue(Wave wave)
    {
        if (wave.Dialogue is null) return;

        GameManager.instance.GamePaused(true);

        DialogueStarted?.Invoke();

        // Bug Starts Here
        dialogueCanvasGroup.alpha = 1f;
        dialogueCanvasGroup.blocksRaycasts = true;
        dialogueText.text = "";
        sentences.Clear();

        Debug.Log("Started Dialogue");

        currentDial = wave.Dialogue;

        sentences.Clear();
        StartText();
    }

    public void StartText()
    {
        StopAllCoroutines();

        foreach (string sentence in currentDial.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            Debug.Log("No more sentences");
            EndText();
            return;
        }

        if (startedTyping)
        {
            Debug.Log("started typing is already true");
            return;
        }

        string newSentence = sentences.Dequeue();

        StopAllCoroutines();

        StartCoroutine(TypeSentence(newSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        startedTyping = true;

        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(letterDisplayDelay);
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, dialogueBlipSFX);
        }

        StartCoroutine(AutoScrollTimer());
        startedTyping = false;
        yield break;
    }

    IEnumerator AutoScrollTimer()
    {
        yield return new WaitForSeconds(autoContinueTime);

        DisplayNextSentence();
        Debug.Log("Display Next");
        yield break;
    }

    public void EndText(bool unPauseGame = true)
    {
        AudioManager.instance.StopSound(AudioManagerChannels.SoundEffectChannel);
        Debug.Log("EndText Called");
        dialogueText.text = "";
        sentences.Clear();
        StopAllCoroutines();

        if(unPauseGame)
            GameManager.instance.GamePaused(false);
        
        startedTyping = false;
        CloseDialogueBox();
    }

    public void ChangeTextSpeed()
    {
        Debug.Log(letterDisplayDelay == minimumValue);
        if (letterDisplayDelay <= minimumValue)
        {
            letterDisplayDelay = maxValue; 
            return;
        }
            
        Debug.Log("test2");
        letterDisplayDelay += letterDisplayDelayOffset;
    }

    public void CloseDialogueBox()
    {
        dialogueCanvasGroup.alpha = 0f;
        dialogueCanvasGroup.blocksRaycasts = false;
        startedTyping = false;
        DialogueOver?.Invoke();
    }
}