using System.Collections.Generic;

namespace AI.NeuralNetwork.Hierarchy
{
    public interface INeuralNetwork
    {
        List<double> Work(List<double> data);
        NeuralEnvironment Environment { get; set; }
    }
}