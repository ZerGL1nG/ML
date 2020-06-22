using System;
using System.Diagnostics;

namespace AI.NeuralNetwork
{
    public enum ActivationFunctions
    {
        Sigmoid,
        Gate,
        Tanh,
        ReLu,
    }

    public static class NetworkManager
    {
        public static Func<double, double> GetActivationFunc(ActivationFunctions func) => func switch
        {
            ActivationFunctions.Gate => (d) => d > 0 ? 1 : 0,
            ActivationFunctions.Sigmoid => (d) => 1 / (1 + Math.Pow(Math.E, -d)),
            ActivationFunctions.Tanh => (d) => 2 / (1 + Math.Pow(Math.E, -2 * d)) - 1,
            ActivationFunctions.ReLu => (d) => d > 0 ? d : 0,
            _ => (d) => d
        };
        
        public static Func<double, double> GetDerivativeFunc(ActivationFunctions func) => func switch
        {
            ActivationFunctions.Gate => (d) => 0,
            ActivationFunctions.Sigmoid => (d) => d * (1 - d),
            ActivationFunctions.Tanh => (d) => 1 / (Math.Cosh(d) * Math.Cosh(d)),
            ActivationFunctions.ReLu => (d) => 1,
            _ => (d) => 1,
        };

    }
}