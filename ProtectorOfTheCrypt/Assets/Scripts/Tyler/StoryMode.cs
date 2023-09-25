using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DialogueController))]
public class StoryMode : GameMode
{
    public WaveManager waveManager;
    public DialogueController DialogueController;

    public void Awake()
    {
        DialogueController = GetComponent<DialogueController>();
    }

    public override bool CheckGameWon()
    {
        return waveManager.state == WaveManager.SpawnState.FINISHED
               && waveManager.EnemySpawner.SpawnedObjects.Count == 0;
    }

    public override bool CheckGameLost()
    {
        return GameManager.instance.Souls <= 0;
    }

    public override void OnGameLost()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public override void OnGameWon()
    {
        Debug.Log("game won test");
        SceneManager.LoadScene("MainMenuScene");
    }
}
