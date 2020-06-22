using System;
using System.Collections.Generic;

namespace TanksGame.Core
{
    public interface IWorld
    {
        public Dictionary<Func<List<double>, int>, int> Evaluate();

    }
}