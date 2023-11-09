using System.Reflection;
using static UnityEngine.ParticleSystem;

public class AddDamageModifier : AbstractValueModifier<float>
{
    public override void Apply(TowerScriptableObject Tower)
    {
        try
        {
            float value = GetAttribute<float>(
                Tower, out object targetObject,
                out FieldInfo field
            );
            value += Amount;
            field.SetValue(targetObject, value);
        }
        catch (InvalidPathSpecifiedException) { } // Log the error
        // So we can fix them
    }
}
