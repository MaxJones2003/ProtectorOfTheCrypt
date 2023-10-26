using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Dialogues")]
public class Dialogue : ScriptableObject
{
    [TextArea(3, 10)]
    public string[] sentences;

}
