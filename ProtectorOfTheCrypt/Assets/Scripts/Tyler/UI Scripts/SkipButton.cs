using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipButton : MonoBehaviour
{
	public void SkipDialogue()
	{
		StoryMode storyMode = GameManager.instance.GameMode as StoryMode;
		Debug.Log("test da thing");
		storyMode.DialogueController.EndText(); // Closes Dialogue Box and resumes game.
	}
}
