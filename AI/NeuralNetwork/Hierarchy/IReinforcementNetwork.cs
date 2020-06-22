using System;

namespace AI.NeuralNetwork.Hierarchy
{
    public interface IReinforcementNetwork : INeuralNetwork
    {
        void Learn(Func<NeuralEnvironment, double> evaluate);
    }
}