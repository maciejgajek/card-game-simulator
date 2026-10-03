public class DealDamageAbility : WarriorAbility
{
    private int damage = 1;

    public override void Execute(GameState state, WarriorInstance warrior, PlayerState owner, PlayerState enemy)
    {
        enemy.TakeDamage(damage);
    }
}
