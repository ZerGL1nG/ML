using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using AI.NeuralNetwork.Hierarchy;
using AI.NeuralNetwork.Neurons;

namespace AI.NeuralNetwork.Networks
{
    public class Perceptron : ISupervisedNetwork
    {
        public NeuralEnvironment Environment { get; set; }
        private List<double> input;
        private double speed;

        public List<double> Work(List<double> data)
        {
            input = data;
            Environment.Work();
            return Environment.GetResults().ToList();
        }
        
        private Func<IEnumerable<IEnumerable<double>>, 
            IEnumerable<IEnumerable<double>>, 
            IEnumerable<double>> _evaluate;


        
        
        public void Teach(Dictionary<List<double>, List<double>> data)
        {
            var count = 0;
            foreach (var (example, answers) in data)
            {
                input = example.ToList();
                Environment.Work();
                var miss = new Dictionary<INeuron, (double, int)>();
                var nextQue = new Dictionary<INeuron, (double, int)>();
                var que = Environment.GetOutputs()
                    .Zip(answers, (n, a) => (n, a))
                    .ToDictionary(t => t.n, t => ((t.a - t.n.Work()), 1));

                while (que.Count != 0)
                {

                    foreach (var (neuron, (sum, num)) in que)
                    {
                        var res = neuron.Work();
                        var q = sum / num;
                        var neuronQ = q * NetworkManager.GetDerivativeFunc(neuron.ActFunc)(res);
                        foreach (var (input, weight) in neuron.GetInputs().ToList())
                        {
                            var delta = speed * neuronQ * input.Work();
                            if (nextQue.ContainsKey(input)) nextQue[input] = (nextQue[input].Item1 + neuronQ * weight, nextQue[input].Item2 + 1);
                            else nextQue.Add(input, (neuronQ * weight,  1));
                            neuron.Connect(input, weight + delta);
                        }  
                    }
                    que = nextQue;
                    nextQue = new Dictionary<INeuron, (double, int)>();
                }
                if(count%10000 == 0)Console.WriteLine(count);
                count++;
            }
            
        }
    }
}