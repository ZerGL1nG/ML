using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AI.NeuralNetwork;
using AI.NeuralNetwork.Algs;

namespace AI
{
    class Program
    {
        static void Main(string[] args)
        {
            
            
        }

        public static void TestGeneticAlgorithm()
        {
            var agents = new List<NeuralEnvironment>();
            var population = 100;
            
            var rand = new Random();
            var dileme = new List<double>();
            var options = 7;
            for (var i = 0; i < options * options * 2; i++)
                dileme.Add(rand.Next(11) / 10d);
            var bacldileme = new List<double>();
            for (var i = 0; i < options * options; i++)
            {
                bacldileme.Add(dileme[i * 2 + 1]);
                bacldileme.Add(dileme[i * 2]);
            }

            for (var i = 0; i < options; i++)
            {
                for (var j = 0; j < options; j++)
                {
                    Console.Write(Math.Round(dileme[2 * (i * options + j)] * 10)
                                  + " " + Math.Round(dileme[2 * (i * options + j) + 1] * 10));
                    if (j+1 != options)
                        Console.Write(" | ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
            
            for (var i = 0; i < population; i++)
                agents.Add(NetworkCreator.Perceptron(8, options, new List<int>() {20}));
            var tournament = new Func<bool, Func<List<NeuralEnvironment>, List<double>>>(print => ((players) =>
            {
                var startElo = 0d;
                var tours = 100;
                var elo = 
                    players.ToDictionary(player => player, player => startElo);

                for (var game = 0; game < tours; game++)
                {
                    //if(print)Console.WriteLine(game);
                    var pairing = new List<(NeuralEnvironment, NeuralEnvironment)>();
                    var list = players.OrderBy(p => -elo[p]).ToList();
                    for (var i = 0; i < players.Count / 2; i++)
                    {
                        var f = list[rand.Next(list.Count)];
                        list.Remove(f);
                        var s = list[rand.Next(list.Count)];
                        list.Remove(s);
                        pairing.Add((f, s));
                    }
                        
                    Parallel.ForEach(pairing, pair =>
                    //foreach(var pair in pairing)
                    {
                        var (player1, player2) = pair;
                        player1.Work(dileme);
                        player2.Work(bacldileme);
                        var firstPlay = player1.GetMaxOutId();
                        var secondPlay = player2.GetMaxOutId();
                        //winwin
                        elo[player1] += dileme[firstPlay * 6 + secondPlay * 2];
                        elo[player2] += dileme[firstPlay * 6 + secondPlay * 2 + 1];
                        /*var E1 = 1 / (1 + Math.Pow(10, (elo[player2] - elo[player1]) / 400));
                        var E2 = 1 / (1 + Math.Pow(10, (elo[player1] - elo[player2]) / 400));
                        elo[player1] += 20 * (res[1] / points + (res[0] / draw) / 2d - E1);
                        elo[player2] += 20 * (res[2] / points + (res[0] / draw) / 2d- E2);*/
                        //}
                    });
                }

                var bestPlayer = elo.OrderBy(p => p.Value).Last().Key;
                if(print)Console.WriteLine(Math.Round(elo[bestPlayer])
                                           + "  "
                                           + Math.Round(elo.Select(p => p.Value).Sum() * 1d / elo.Count));
                return players.Select(p => elo[p]).ToList(); 
            }));
            
            
            
            var iterations = 1000;
            for (var i = 0; i < iterations; i++)
            {
                agents = GeneticAlgorithm.Improve(
                    agents,
                    GeneticAlgorithm.RandomMerge,
                    tournament(false));
            }

            foreach(var agent in agents.Take(10))
            {
                agent.Work(dileme);
                Console.Write(agent.GetMaxOutId() + " ");
                agent.Work(bacldileme);
                Console.WriteLine(agent.GetMaxOutId());
            }
        }


        public static void TestBackpropagation()
        {
            var NN = NetworkCreator.Perceptron(2, 1, new []{20, 20, 20});

            var nums = 100000;
            var data = new Dictionary<List<double>, List<double>>();
            var rand = new Random();
            var dif = 1000;
            var shell = 500;
            for (var i = 0; i < nums; i++)
            {
                var first = rand.Next(dif) - shell;
                var second = rand.Next(dif) - shell;
                data.Add(new List<double>(){ first, second },
                    new List<double>()
                    {
                        (first + second + 2 * shell) / 2d / dif,
                    });   
            }
            
            
            BackpropagationAlgorithm.Teach(NN, data);
            NN.Print();

            for (var i = 0; i < 100; i++)
            {
                var first = rand.Next(dif) - shell;
                var second = rand.Next(dif) - shell;
                NN.Work(new List<double>() {first, second});
                var res = NN.GetResults()[0];
                Console.WriteLine(first + "  " + second + " . " + (first + second) + " " + (Math.Round((res * dif - shell) * 2)));
                Console.ReadLine();
            }
        }
    }
}