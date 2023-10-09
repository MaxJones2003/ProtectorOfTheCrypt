using System.Reflection;
using UnityEngine;

public class SubtractFireRateModifier : AbstractValueModifier<float>
{
    public override void Apply(TowerScriptableObject Tower)
    {
        try
        {
            float value = GetAttribute<float>(
                Tower, out object targetObject,
                out FieldInfo field
            );

            value -= Amount;
            value = Mathf.Max(value, 0.1f);
            
            field.SetValue(targetObject, value);
        }
        catch (InvalidPathSpecifiedException) {} // Log the error
        // So we can fix them
    }
}
