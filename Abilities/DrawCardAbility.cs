public class DrawCardAbility : MageAbility
{
    private int cardsToDraw = 1;

    public override void Execute(GameState state, MageInstance mage, PlayerState owner, PlayerState enemy)
    {
        owner.DrawCards(cardsToDraw);
    }
}
