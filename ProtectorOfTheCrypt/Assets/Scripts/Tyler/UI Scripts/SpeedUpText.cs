using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpText : MonoBehaviour
{

    public void ChangeSpeed()
    {
        StoryMode storyMode = GameManager.instance.GameMode as StoryMode;
        storyMode.DialogueController.ChangeTextSpeed();
    }
}
