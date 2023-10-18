using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDialogueBox : MonoBehaviour
{
    
    public void CloseDialgoueManager()
    {
        if (GameManager.instance.TryGetComponent<DialogueController>(out DialogueController dialogueController) 
            && dialogueController.startedTyping)
            dialogueController.EndText(false);
    }
}
