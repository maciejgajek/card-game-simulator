public class HealSpecialAbility : SpecialAbility
{
    private int healAmount = 10;

    public int HealAmount => healAmount;

    public override void Activate(GameState state, PlayerState caster) => caster.Heal(healAmount);
}
