using System.Collections;
using System.Collections.Generic;

namespace AI.NeuralNetwork.Hierarchy
{
    public interface ISupervisedNetwork : INeuralNetwork
    {
        void Teach(Dictionary<List<double>, List<double>> data);
    }
}