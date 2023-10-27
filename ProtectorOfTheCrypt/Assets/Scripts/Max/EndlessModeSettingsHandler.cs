using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessModeSettingsHandler : MonoBehaviour
{
    public MapSizeSettings easyMap;
    public MapSizeSettings standardMap;
    public MapSizeSettings hardMap;
    public EnemyDifficultySettings easyEnemy;
    public EnemyDifficultySettings standardEnemy;
    public EnemyDifficultySettings hardEnemy;

    public MapSizeSettings currentMapValue;
    public EnemyDifficultySettings currentEnemyValue;

    public string seed;



    public void Ready()
    {
        EndlessModeSettings settings = new(seed, currentMapValue, currentEnemyValue);

        EndlessMode mode = GameManager.instance.GameMode as EndlessMode;
        mode.ReadyToLoadMap(settings);
    }
}
