using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AI.NeuralNetwork.Neurons;


namespace AI.NeuralNetwork
{
    public class NeuralEnvironment
    {
        public List<Neuron> Neurons { get; set; }
        public List<Neuron> Output { get; set; }
        public List<Neuron> Input { get; set; }

        public Dictionary<string, Neuron> NeuronsIds;
        public NeuralEnvironment()
        {
            NeuronsIds = new Dictionary<string, Neuron>();
            Neurons= new List<Neuron>();
            Output = new List<Neuron>();
            Input = new List<Neuron>();
        }

        public void Save(string output)
        {
            string jsonString;
            //File.Delete(output);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            jsonString = JsonSerializer.Serialize(this, options);
            File.WriteAllText(output, jsonString);
        }


        public void AddNeuron(Neuron neuron, IEnumerable<Neuron> inputs)
        {
            if(neuron.Tag == NeuronTag.output) Output.Add(neuron);
            if(neuron.Tag == NeuronTag.input) Input.Add(neuron);
            var rand = new Random();
            neuron.Id = Neurons.Count.ToString();
            NeuronsIds[neuron.Id] = neuron;
            Neurons.Add(neuron);
            foreach (var input in inputs)
                if(Neurons.Contains(input))
                    neuron.Connect(input.Id, rand.NextDouble() - 0.5d);//wights generation to fix
        }

        public bool Connect(Neuron neuron, Neuron input, double weight)
        {
            if (!Neurons.Contains(neuron) || !Neurons.Contains(input)) return false;
            neuron.Connect(input.Id, weight);
            return true;
        }

        public bool Disconnect(Neuron neuron, Neuron input)
        {
            if (!Neurons.Contains(neuron) || !Neurons.Contains(input)) return false;
            neuron.Disconnect(input.Id);
            if (Output.Contains(neuron)) Output.Remove(neuron);
            return true;
        }

        public NeuralEnvironment Clone()
        {
            var newEnvironment = new NeuralEnvironment();
            //var neuronCopies = new Dictionary<INeuron, INeuron>();
            foreach (var neuron in Neurons)
                newEnvironment
                    .AddNeuron(
                        new Neuron(
                            neuron.ActFunc,
                            neuron.Tag,
                            new Dictionary<string, double>(neuron.Inputs)),
                        new List<Neuron>());
            
            /*foreach (var neuron in Neurons)
                foreach (var (input, weight) in neuron.Inputs)
                    newEnvironment.Connect(neuronCopies[neuron], neuronCopies[NeuronsIds[input]], weight);*/
            return newEnvironment;
        }
        
        
        public void Print()
        {
            var que = new Queue<Neuron>();
            //number of neurons
            Console.WriteLine(Neurons.Count + "");
            Console.WriteLine();
            //inputs
            Console.WriteLine(Input.Select(neuron => "" + neuron.Id));
            Console.WriteLine();
            foreach (var output in Output) que.Enqueue(output);
            while (que.Count != 0)
            {
                var neuron = que.Dequeue();
                Console.WriteLine();
                Console.WriteLine(neuron.Id);
                foreach (var (input, weight) in neuron.Inputs)
                {
                    Console.WriteLine(NeuronsIds[input] + " " + weight);
                    if(!que.Contains(NeuronsIds[input])) que.Enqueue(NeuronsIds[input]);
                }
            }
        }

        public List<Neuron> GetOutputs() => Output;
        public List<Neuron> GetNeurons() => Neurons;
        public List<double> GetResults() => Output.Select(n => n.Work(NeuronsIds)).ToList();

        public int GetMaxOutId()
        {
            return Output.IndexOf(new List<Neuron>(Output).OrderBy(p => p.Work(NeuronsIds)).Last());
        }

        public List<double> Work(List<double> data)
        {
            foreach (var neuron in Neurons)
                neuron.Reset();
            foreach (var (d, inputNeuron) in data.Zip(Input, (i, n) => (i, n)))
                inputNeuron.GetInput(d);
            foreach (var neuron in Output)
                neuron.Work(NeuronsIds);
            return GetResults();
        }

            public void InitIds()
            {
                foreach (var neuron in Neurons)
                {
                    NeuronsIds.Add(neuron.Id, neuron);
                }

                Output = Neurons.Where(n => n.Tag == NeuronTag.output).ToList();
                Input = Neurons.Where(n => n.Tag == NeuronTag.input).ToList();
                
                
                Output = Output.OrderBy(o => int.Parse(o.Id)).ToList();
                Input = Input.OrderBy(o => int.Parse(o.Id)).ToList();
            }
    }
}