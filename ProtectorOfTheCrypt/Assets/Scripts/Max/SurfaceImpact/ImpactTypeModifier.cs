public class ImpactTypeModifier : AbstractValueModifier<ImpactType>
{
    public override void Apply(TowerScriptableObject Tower)
    {
        Tower.ImpactType = Amount;
    }
}
