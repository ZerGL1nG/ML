using System;
using System.Collections.Generic;

namespace TanksGame.Core
{
    public class PersonalGameState : IPersonalGameState
    {
        private readonly int _bullets;
        private readonly int _oil;
        private readonly IEnumerable<double> _cells;

        public List<double> ListData()
        {
            var data = new List<double>();
            data.AddRange(_cells);
            data.Add(_oil * 1d / GameRules.MaxOil);
            data.Add(_bullets * 1d / GameRules.MaxBullets);
            return data;
        }


    public PersonalGameState(IEnumerable<double> cells, int oil, int bullets)
        {
            this._cells = cells;
            _oil = oil;
            _bullets = bullets;
        }
    }
}