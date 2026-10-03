using System.Collections.Generic;
public class GameSetup
{
    public List<CardData>  PlayerDeck      = new List<CardData>();
    public PlayerEquipment PlayerEquipment = new PlayerEquipment();
    public StartingStats   PlayerStats     = new StartingStats();
    public List<CardData>  EnemyDeck       = new List<CardData>();
    public PlayerEquipment EnemyEquipment  = new PlayerEquipment();
    public StartingStats   EnemyStats      = new StartingStats();

    public PlayerEquipment EquipmentFor(bool isEnemy) => isEnemy ? EnemyEquipment : PlayerEquipment;
    public List<CardData>  DeckFor(bool isEnemy)      => isEnemy ? EnemyDeck      : PlayerDeck;
    public StartingStats   StatsFor(bool isEnemy)     => isEnemy ? EnemyStats     : PlayerStats;
}
