using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using System.Xml;
using AI.NeuralNetwork;
using AI.NeuralNetwork.Algs;
using TanksGame.Core;
using TanksGame.Core.Constructions;
using TanksGame.Core.GameObjects;
using TanksGame.Core.GameProcess;

namespace TanksGame
{
    class Program
    {
        private static List<int> _actions;

        static void Main(string[] args)
        {
            // All in-game parameters can be changed in GameRules.cs  (but may cause some exceptions)
            
            // Example of using library and making / teaching bot to play a game
            // NetworkCreator.Perceptron(inputs, outputs, list of neurons in hidden layers) - to create agent
            // NetworkCreator.ReadFromFile(filename) - to load agent from file
            
            // PlayGame(agents) - to watch agents in action
            // Teach(agents, number of example games, number of learning cycles) - to teach agents with backpropagation algorithm
            // Gen(agents, number of generations) - to improve agents with genetic algorithm
            
            
            
            
            
            // Shows perfect average lifetime for agents in game with current values
            _actions = new List<int>();
            for(var i = 0; i < GameRules.NumberOfActions; i++)
                _actions.Add(0);
            Console.WriteLine("Average target = " + (GameRules.StartOil
                                                     + GameRules.OilProb
                                                     * GameRules.OilBonus
                                                     * GameRules.MapWidth
                                                     * GameRules.MapHeight
                                                     / GameRules.numberOfAgents));
            Console.WriteLine();
            Console.WriteLine();
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            // Code Examples
            
            
            /*
            //Creates new agents
            
            var inputs = ((GameRules.SightWidth * 2 + 1) * (GameRules.SightHeight) - 1) * GameRules.Tags.Count + 2;
            var outputs = GameRules.NumberOfActions;
            var neuronsInHiddenLayers = new List<int>() {20};
            var agents = new List<NeuralEnvironment>();
            for(var i = 0; i < GameRules.numberOfAgents; i++)
                agents.Add(NetworkCreator.Perceptron(inputs, outputs, neuronsInHiddenLayers ));
            */
            
            /*
            //Get saved agents from files
            
            var filename = "agent";
            var iter = GameRules.numberOfAgents;
            var agents = new List<NeuralEnvironment>();
            for(var i = 0; i < iter; i++)
                agents.Add(NetworkCreator.ReadFromFile(filename + i));
            */
            
            /*
            //Shows game with agents  (press Enter to see next game tick)
            
            PlayGame(agents, true);
            */
            

            /*
            //Teach agents by playing yourself versus them.
            
            var agents1 = Teach(agents, 1, 10);
            */

            /*
            //Agents learn playing against each other
            
            var agents1 = Gen(agents, 100);
            */
            
            
            /*
            //Save agents
            var filename = "agent";
            iter = 0;
            foreach (var agent in agents)
                agent.Save(filename + iter++);
            */
            
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Here starts actual code (loads pretty fine agents tho)
            
            
            /*var filename = "agent";
            var iter = GameRules.numberOfAgents;
            var agents = new List<NeuralEnvironment>();
            for(var i = 0; i < iter; i++)
                agents.Add(NetworkCreator.ReadFromFile(filename + i));
            */
            
            //loaded agents
            var filename = "agent";
            var iter = GameRules.numberOfAgents;
            var agents = new List<NeuralEnvironment>();
            for(var i = 0; i < iter; i++)
                agents.Add(NetworkCreator.ReadFromFile(filename + i));
            
            //let'em learn for 10 generations and watch their averages
            agents = Gen(agents, 10);
            //trying to teach'em
            agents = Teach(agents, 3, 5);
            //10 more generation to see the affect of us being a poor teacher
            agents = Gen(agents, 10);

            //probly don't wanna save that
            /*iter = 0;
            foreach (var agent in agents)
                agent.Save(filename + iter++);
            */
            
            Console.WriteLine("Finished");
            Console.ReadLine();
        }


        
        
        
        
        
        private static List<NeuralEnvironment> Teach(List<NeuralEnvironment> agents, int tics, int iter)
        {
            for (var i = 0; i < tics; i++)
            {
                var data = new Dictionary<List<double>, List<double>>();
                var braaaainz = agents.Select(UseNetwork).ToList();
                var myBraaainz = new Func<List<double>, int>(state =>
                {
                    Console.Clear();
                    var it = 0;
                    var objects = new Dictionary<(int, int), string>();
                    for (var t = 0; t < GameRules.Tags.Count; t++)
                    {
                        for (var x = -GameRules.SightWidth; x <= GameRules.SightWidth; x++)
                        {
                            for (var y = 0; y < GameRules.SightHeight; y++)
                            {
                                if (x == 0 && y == 0) continue;
                                if (t == 0)
                                    objects[(x, y)] = " ";
                                if (Math.Abs(state[it]) > 0.1)
                                    objects[(x, y)] = t switch
                                    {
                                        0 => "¤",
                                        1 => "Ш",
                                        2 => "#",
                                        3 => Math.Round(state[it] * 4) switch
                                        {
                                            1 => "v",
                                            2 => "<",
                                            3 => "^",
                                            4 => ">",
                                            _ => "?",
                                        },
                                        _ => "?",
                                    };
                                it++;
                            }
                        }
                    }

                    for (var y = GameRules.SightHeight - 1; y >= 0; y--)
                    {
                        for (var x = -GameRules.SightWidth; x <= GameRules.SightWidth; x++)
                        {
                            if (x == 0 && y == 0)
                                Console.Write("^ ");
                            else
                                Console.Write(objects[(x, y)] + " ");
                        }
                        Console.WriteLine();
                    }

                    var oil = Math.Round(state[it++] * GameRules.MaxOil);
                    var bullets = Math.Round(state[it] * GameRules.MaxBullets);
                    Console.WriteLine($"Oil {oil}. Bullets {bullets}");
                    Console.WriteLine("To go forward and turn - use W, A,D.  To shoot use Space, anything else to stay.");
                    Console.WriteLine();

                    var key = 1;
                    var gotKey = false;
                    while (!gotKey)
                    {
                        if(!Console.KeyAvailable)
                            continue;
                        key = Console.ReadKey(true).Key switch
                        {
                            ConsoleKey.A => 3,
                            ConsoleKey.D => 4,
                            ConsoleKey.W => 2,
                            ConsoleKey.Spacebar => 0,
                            _ => 1,
                        };
                        gotKey = true;
                    }

                    var answer = key;
                    var answerList = new List<double>();
                    for (var act = 0; act < GameRules.NumberOfActions; act++)
                        answerList.Add(0);
                    answerList[answer] = 1;
                    data.Add(state, answerList);
                    return answer;
                });
                
                braaaainz.Add(myBraaainz);
                var units = braaaainz.Select(agent => new Tank(agent)).ToList();
                var map = GameRules.CreateNewMap(units);
                var world = new TankWorld(units, map);
                world.Evaluate();
                Console.WriteLine("Game over. Applying received data.");
                for (var s = 0; s < iter; s++)
                {
                    Parallel.ForEach(agents, (e) => BackpropagationAlgorithm.Teach(e, data));
                    Console.WriteLine($"Progress {s+1} of {iter}");
                }
            }

            return agents;
        }


        private static List<NeuralEnvironment> Gen(List<NeuralEnvironment> agents, int tics = 10000, double avgt = 1000, double maxt = 3000, int set = 10)
        {//Genetic
            var i = 0;
            var avg = 0d;
            var max = 0d;
            Console.WriteLine("Number of actions : shoot|stay|go forward|turn left|turn right");
            while (i < tics && avg < avgt && max < maxt)
            {
                i++;
                var agents1 = new List<NeuralEnvironment>(agents);
                agents = GeneticAlgorithm.Improve(agents,
                    GeneticAlgorithm.RandomMerge,
                    (players) =>
                    {
                        var results = new List<double>();
                        foreach (var agent in agents)
                            results.Add(0);

                        Parallel.For(0, set, (iter) =>
                        {
                            var res = PlayGame(agents1);
                            lock(results)
                                results = results.Zip(res, (r, g) => r + g).ToList();
                        });
                        //for (var j = 0; j < set; j++)
                          //  results = results.Zip(PlayGame(agents1), (r, g) => r + g).ToList();

                        avg = results.Sum() / results.Count / set;
                        max = results.OrderBy(r => r).Last() / set;
                        Console.Write($"{_actions[0], 5}|{_actions[1], 5}|{_actions[2], 5}|{_actions[3], 5}|{_actions[4], 5}     ");
                        Console.WriteLine($"Average {Math.Round(avg)}   Max {Math.Round(max)}");
                        //Console.ReadLine();
                        for(var i = 0; i < GameRules.NumberOfActions; i++)
                            _actions[i] = 0;
                        return results;
                    });
            }

            return agents;
        }
        
        

        private static Func<List<double>, int> UseNetwork(NeuralEnvironment environment)
        {
            return new Func<List<double>, int>((data) =>
            {
                environment.Work(data);
                var id= environment.GetMaxOutId();
                _actions[id]++;
                return id;
            });
        }


        private static List<double> PlayGame(List<NeuralEnvironment> agents1, bool show = false)
        {
            var braaaainz = agents1.Select(UseNetwork).ToList();
            var units = braaaainz.Select(agent => new Tank(agent)).ToList();
            var map = GameRules.CreateNewMap(units);
            var world = new TankWorld(units, map, draw: show);
            var dict = world.Evaluate();
            var result = dict.Values.Select(p => (double)(p)).ToList();
            return result;
        }
    }
}