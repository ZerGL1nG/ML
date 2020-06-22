using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using AI.NeuralNetwork.Neurons;

namespace AI.NeuralNetwork
{
    public static class NetworkCreator
    {
        public static NeuralEnvironment Perceptron(int inputs,
            int outputs,
            IEnumerable<int> hiddenLayers)
        {
            var environment = new NeuralEnvironment();
            const ActivationFunctions func = ActivationFunctions.Sigmoid;
            var actFun = NetworkManager.GetActivationFunc(func);
            //input
            var prev = new List<Neuron>();
            var cur = new List<Neuron>();
            for (var i = 0; i < inputs; i++)
            {
                var i1 = i;
                var neuron = new Neuron((ActivationFunctions)(-1), NeuronTag.input);
                prev.Add(neuron);
                environment.AddNeuron(neuron, new List<Neuron>());
            }
            //hidden
            foreach (var layerSize in hiddenLayers)
            {
                for (var i = 0; i < layerSize; i++)
                {
                    var neuron = new Neuron(ActivationFunctions.Sigmoid);
                    cur.Add(neuron);
                    environment.AddNeuron(neuron, prev);
                }

                prev = cur;
                cur = new List<Neuron>();
            }
            //output
            for (var i = 0; i < outputs; i++)
            {
                //var neuron = new SimpleNeuron((ActivationFunctions) (-1));
                var neuron = new Neuron(ActivationFunctions.Sigmoid, NeuronTag.output);

                environment.AddNeuron(neuron, prev);
            }

            return environment;
        }



        public static NeuralEnvironment ReadFromFile(string input)
        {
            var res = JsonSerializer.Deserialize<NeuralEnvironment>(File.ReadAllText(input));
            res.InitIds();
            return res;
        }

    }
}