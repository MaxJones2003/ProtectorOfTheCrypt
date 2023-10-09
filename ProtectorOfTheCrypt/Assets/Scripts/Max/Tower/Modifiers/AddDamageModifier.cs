using System.Reflection;
using static UnityEngine.ParticleSystem;

public class AddDamageModifier : AbstractValueModifier<float>
{
    public override void Apply(TowerScriptableObject Tower)
    {
        try
        {
            MinMaxCurve damageCurve = GetAttribute<MinMaxCurve>(
                Tower, out object targetObject,
                out FieldInfo field
            );

            switch(damageCurve.mode)
            {
                case UnityEngine.ParticleSystemCurveMode.TwoConstants:
                    damageCurve.constantMin += Amount;
                    damageCurve.constantMax += Amount;
                    break;
                case UnityEngine.ParticleSystemCurveMode.TwoCurves:
                    damageCurve.curveMultiplier += Amount;
                    break;
                case UnityEngine.ParticleSystemCurveMode.Constant:
                    damageCurve.constant += Amount;
                    break;
            }

            field.SetValue(targetObject, damageCurve);
        }
        catch (InvalidPathSpecifiedException) {} // Log the error
        // So we can fix them
    }
}
