using System;
using System.Collections.Generic;
using System.Linq;

namespace TanksGame.Core.GameObjects
{
    public class Tank : IUnit
    {
        public Func<List<double>, int> Decision;
        public string getTag() => "Tank";
        public IObject Clone() => this;

        public UnitActions GetAction(IPersonalGameState state) => 
            GameRules.ParseAction(Decision(state.ListData()));
        public int Direction { get; set; }

        public Tank(Func<List<double>, int> dec)
        {
            Decision = dec;
            Direction = 0;
        }
    }
}