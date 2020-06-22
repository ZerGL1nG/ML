using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AI.NeuralNetwork;

namespace AI.NeuralNetwork.Neurons
{

    public enum NeuronTag
    {
        input,
        output,
        hidden,
    }
    
    public class Neuron
    {
        public string Id { get; set; }
        public bool Set { get; set; }
        public ActivationFunctions ActFunc { get; set; }
        public Dictionary<string, double> Inputs { get; set; }
        public double Result { get; set; }

        public NeuronTag Tag { get; set; }

        
        
        public Neuron()
        {
            Inputs = new Dictionary<string, double>();
            ActFunc = ActivationFunctions.Sigmoid;
            Set = false;
            Id = "";
            Set = false;
            Result = 0;
            Tag = NeuronTag.hidden;
        }
        
        public Neuron(ActivationFunctions func, NeuronTag tag = NeuronTag.hidden)
        {
            Inputs = new Dictionary<string, double>();
            ActFunc = func;
            Set = false;
            Id = "";
            Set = false;
            Result = 0;
            Tag = tag;
        }
        
        public Neuron(ActivationFunctions func, NeuronTag tag, Dictionary<string, double> inputs)
        {
            Inputs = new Dictionary<string, double>(inputs);
            ActFunc = func;
            Set = false;
            Id = "";
            Set = false;
            Result = 0;
            Tag = tag;
        }
        
        
        public double Work(Dictionary<string, Neuron> neurons)
        {
            if (Set) return Result;
            Result = 0;
            foreach (var (input, weight) in Inputs)
            {
                if (!neurons.ContainsKey(input))
                    return 0;
                Result += neurons[input].Work(neurons) * weight;
            }

            var t = NetworkManager.GetActivationFunc(ActFunc)(Result);
            Set = true;
            Result = t;
            return Result;
        }
        
        
        public void Connect(string id, double weight)
        {
            Inputs[id] = weight;
        }

        public void Disconnect(string id)
        {
            Inputs.Remove(id);
        }

        public void Reset()
        {
            Set = false;
        }

        public void GetInput(double input)
        {
            Set = true;
            Result = input;
        }
    }
}