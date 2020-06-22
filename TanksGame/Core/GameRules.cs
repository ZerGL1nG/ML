using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using TanksGame.Core.Constructions;
using TanksGame.Core.GameObjects;

namespace TanksGame.Core
{
    public static class GameRules
    {
        public const int NumberOfActions = 5;


        //Changeable
        public const int numberOfAgents = 100;
        public const int SightHeight = 4;
        public const int SightWidth = 2;
        public const int MaxBullets = 100;
        public const int MaxOil = 1000;
        public const int MapHeight = 30;
        public const int MapWidth = 80;

        public const double OilProb = 0.3;
        public const double BulletsProb = 0.1; //0.1;
        public const double WallProb = 0.2; //0.3;
        public const int OilBonus = 10;
        public const int BulletsBonus = 3;
        public const int ShootingRange = 4;

        public const int StartBullets = 3;
        public const int StartOil = 10;


        //some base game funcs

        private static readonly Dictionary<int, UnitActions> Actions = new Dictionary<int, UnitActions>()
        {
            {0, UnitActions.Shoot},
            {1, UnitActions.Stay},
            {2, UnitActions.MoveForward},
            {3, UnitActions.TurnLeft},
            {4, UnitActions.TurnRight},
        };

        public static UnitActions ParseAction(int action) => Actions[action];

        public static IPersonalGameState GetData(IUnit obj, MapState map, int oil, int bullets)
        {
            var list = new List<double>();
            var objCoord = map.GetCoordinates(obj);
            var dir = ((IUnit) obj).Direction;
            foreach(var tag in Tags)
            {
                for (var i = -SightWidth; i < SightWidth + 1; i++)
                {
                    for (var j = 0; j < SightHeight; j++)
                    {
                        if (i == 0 && j == 0) continue;
                        //find coordinates of needed cell
                        var coord = CalculateRelativeCoordinates(objCoord, new Coordinates(i, j, true), dir);
                        var tobj = map.GetObject(coord);
                        if (tobj.getTag() == "Tank" && tag == "Tank")
                        {
                            var redir = GetRelativeDirection(((Tank) (tobj)).Direction, -dir);
                            list.Add((redir + 1) / 4d);
                        }
                        else
                            list.Add(tobj.getTag() == tag ? 1 : 0);
                    }
                }
            }

            return new PersonalGameState(list, oil, bullets);
        }
        
        
        
        
        public static Coordinates CalculateRelativeCoordinates(Coordinates coord, Coordinates delta, int direction)
        {
            return direction switch
            {
                0 => new Coordinates(coord.X - delta.X, coord.Y - delta.Y),
                1 => new Coordinates(coord.X - delta.Y, coord.Y + delta.X),
                2 => new Coordinates(coord.X + delta.X, coord.Y + delta.Y),
                3 => new Coordinates(coord.X + delta.Y, coord.Y - delta.X),
                _ => new Coordinates(0, 0)
            };
        }
        
        public static List<string> Tags = new List<string>()
        {
            "Oil",
            "Bullets",
            "Wall",
            "Tank",
        };

        public static int GetRelativeDirection(int toMe, int meToMap)
        {
            return ((toMe + 4) % 4) switch
            {
                0 => (meToMap + 6) % 4,
                1 => (meToMap + 7) % 4,
                2 => (meToMap + 4) % 4,
                3 => (meToMap + 5) % 4,
                _ => 0
            };
        }
        
        public static MapState CreateNewMap(IEnumerable<IUnit> units)
        {
            var rand = new Random();
            var map = new MapState();
            
            //Added Base Stuff
            for (var i = 0; i < MapWidth; i++)
            {
                for (var j = 0; j < MapHeight; j++)
                {
                    var t = rand.NextDouble();
                    if (t < OilProb) map.AddObject(new Oil(), new Coordinates(i, j));
                    else if (t < OilProb + BulletsProb) map.AddObject(new Bullets(), new Coordinates(i, j));
                    else if (t < OilProb + BulletsProb + WallProb) map.AddObject(new Wall(), new Coordinates(i, j));
                }
            }
            
            //Spread all units across the map
            foreach (var unit in units)
            {
                var coords = new Coordinates(rand.Next(MapWidth), rand.Next(MapHeight));
                while (map.HasCoordinates(coords) && map.GetObject(coords).getTag() == "Tank")
                    coords = new Coordinates(rand.Next(MapWidth), rand.Next(MapHeight));
                map.AddObject(unit, coords);
            }
            return map;
        }
    }
}