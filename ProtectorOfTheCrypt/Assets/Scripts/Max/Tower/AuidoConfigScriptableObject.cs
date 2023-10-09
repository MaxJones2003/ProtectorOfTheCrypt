using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Contains all of the sounds that can be played by a tower. This includes an array of sounds that can be made on Shoot and a sound that can be played on the placement of the tower.
/// </summary>
[CreateAssetMenu(fileName = "Audio Config", menuName = "Towers/AudioConfig", order = 5)]
public class AuidoConfigScriptableObject : ScriptableObject, System.ICloneable
{
    [Range(0,1f)] public float Volume = 1;
    public AudioClip[] FireClips;
    public AudioClip BuildingClip;

    public void PlayShootingClip(AudioSource AudioSource)
    {
        AudioSource.PlayOneShot(FireClips[Random.Range(0, FireClips.Length)], Volume);
    }

    public void PlayBuildingClip(AudioSource AudioSource)
    {
        AudioSource.PlayOneShot(BuildingClip, Volume);
    }
    public object Clone()
    {
        AuidoConfigScriptableObject config = CreateInstance<AuidoConfigScriptableObject>();
        Utilities.CopyValues(this, config);
        return config;
    }
}
