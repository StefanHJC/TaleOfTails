namespace Core.Unit.Command
{
    public enum CommandTypeId
    {
        Move,
        Rotate,
        Attack,
        UseAbility,
        SkipTurn
    }

    public enum AttackTypeId
    {
        Melee,
        Ranged
    }
}