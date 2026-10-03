public class EndOfTurnHealPassive : CreaturePassive
{
    private int healAmount = 1;

    public int HealAmount => healAmount;

    public override void OnTurnEnd(PlayerState owner) => owner.Heal(healAmount);
}
