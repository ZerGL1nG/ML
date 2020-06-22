using System.Collections;
using System.Collections.Generic;

namespace TanksGame.Core
{
    public interface IAgent
    {
        int MakeChoice(List<double> data);
        //Remove to fix
    }
}