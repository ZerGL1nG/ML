using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AI.NeuralNetwork.Algs
{
    public static class GeneticAlgorithm
    {
        private const double PercentKept = 0.15d;
        
        public static List<NeuralEnvironment> Improve(List<NeuralEnvironment> environments,
            Func<NeuralEnvironment, NeuralEnvironment, NeuralEnvironment> mergeGenes,
            Func<List<NeuralEnvironment>, List<double>> evaluate)
        {
            var population = environments.Count;
            var values = evaluate(environments);
            var evaluates = new Dictionary<NeuralEnvironment, double>();
            foreach(var (environment, value) in environments.Zip(values, (e, v) => (e, v)))
                evaluates.Add(environment, value);
            var bestT = evaluates
                .ToList()
                .OrderBy(p => -p.Value)
                .Take((int) (population * PercentKept))
                .ToList();

            var best = bestT.Select(t => t.Key).ToList();
            var newParticipants = new List<NeuralEnvironment>();
            var iter = 0;
            var rand = new Random();
            for (var i = best.Count; i < population; i++)
            {
                newParticipants.Add(mergeGenes(best[iter], best[rand.Next(best.Count)]));
                iter = (iter + 1) % (best.Count - 1);
            }
            best.AddRange(newParticipants);
            return best;
        }
        
        
        
        

        public const double MergeCoeff = 0.5d;
        public const double MutationProbability = 0.01d;
        
        public static NeuralEnvironment RandomMerge(NeuralEnvironment first, NeuralEnvironment second)
        {
            var rand = new Random();
            var newGene = first.Clone();
            foreach (var neuron in second.GetNeurons())
            {
                var id = neuron.Id;
                var newNeuron = newGene.NeuronsIds[id];
                foreach (var (input, weight) in neuron.Inputs)
                {
                    if (rand.NextDouble() > MergeCoeff)
                        newGene.Connect(newNeuron, newGene.NeuronsIds[input], weight);
                    if (rand.NextDouble() < MutationProbability)
                        newGene.Connect(newNeuron, newGene.NeuronsIds[input],
                            newNeuron.Inputs[input] + (rand.NextDouble()*2 - 1));
                            //(rand.NextDouble() * 2 - 1));
                }
            }
            return newGene;
        }
    }
}