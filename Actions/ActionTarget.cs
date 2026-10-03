using System.Collections.Generic;

public readonly struct ActionTarget
{
    public readonly int? PlayerIndex;
    public readonly int? CreatureIndex;
    public readonly int? BuildingIndex;

    public ActionTarget(int? playerIndex, int? creatureIndex = null, int? buildingIndex = null)
    {
        PlayerIndex   = playerIndex;
        CreatureIndex = creatureIndex;
        BuildingIndex = buildingIndex;
    }

    public static ActionTarget None => default;
    public static ActionTarget Player(int playerIdx)                    => new ActionTarget(playerIdx);
    public static ActionTarget Creature(int playerIdx, int creatureIdx) => new ActionTarget(playerIdx, creatureIdx, null);
    public static ActionTarget Building(int playerIdx, int buildingIdx) => new ActionTarget(playerIdx, null, buildingIdx);

    public bool IsNone     => !PlayerIndex.HasValue;
    public bool IsPlayer   => PlayerIndex.HasValue && !CreatureIndex.HasValue && !BuildingIndex.HasValue;
    public bool IsCreature => PlayerIndex.HasValue && CreatureIndex.HasValue;
    public bool IsBuilding => PlayerIndex.HasValue && BuildingIndex.HasValue;
}
