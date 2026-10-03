public class TowerAura : Aura
{
    private int charismaAmount = 1;

    public override void OnEnterBoard(PlayerState owner)
    {
        owner.AddCharisma(charismaAmount);
    }

    public override void OnLeaveBoard(PlayerState owner)
    {
        owner.RemoveCharisma(charismaAmount);
    }
}
