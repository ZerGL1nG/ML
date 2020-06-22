using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using AI.NeuralNetwork.Neurons;

namespace AI.NeuralNetwork.Algs
{
    public static class BackpropagationAlgorithm
    {
        public static void Teach(NeuralEnvironment environment, Dictionary<List<double>, List<double>> data)
        {
            var count = 0;
            var speed = 0.7;
            foreach (var (example, answers) in data)
            {
                environment.Work(example);
                var miss = new Dictionary<Neuron, (double, int)>();
                var nextQue = new Dictionary<Neuron, (double, int)>();
                var que = environment.GetOutputs()
                    .Zip(answers, (n, a) => (n, a))
                    .ToDictionary(t => t.n, t => ((t.a - t.n.Work(environment.NeuronsIds)), 1));

                while (que.Count != 0)
                {

                    foreach (var (neuron, (sum, num)) in que)
                    {
                        var res = neuron.Work(environment.NeuronsIds);
                        var q = sum / num;
                        var neuronQ = q * NetworkManager.GetDerivativeFunc(neuron.ActFunc)(res);
                        foreach (var (input, weight) in neuron.Inputs.ToList())
                        {
                            var inputNeuron = environment.NeuronsIds[input];
                            var delta = speed * neuronQ * inputNeuron.Work(environment.NeuronsIds);
                            if (nextQue.ContainsKey(inputNeuron)) nextQue[inputNeuron] 
                                = (nextQue[inputNeuron].Item1 + neuronQ * weight, nextQue[inputNeuron].Item2 + 1);
                            else nextQue.Add(inputNeuron, (neuronQ * weight,  1));
                            neuron.Connect(input, weight + delta);
                        }  
                    }
                    que = nextQue;
                    nextQue = new Dictionary<Neuron, (double, int)>();
                }
            }
            
        }
    }
}