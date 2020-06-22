using System.Data;

namespace TanksGame.Core
{
    public interface IUnit : IObject
    {
        UnitActions GetAction(IPersonalGameState state);
        int Direction { get; set; }
    }
}