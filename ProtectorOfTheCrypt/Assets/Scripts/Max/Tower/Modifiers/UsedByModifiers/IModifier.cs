using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifier
{
    void Apply(TowerScriptableObject Tower);
}
