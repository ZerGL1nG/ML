using System;
using System.Collections.Generic;
using System.Linq;
using TanksGame.Core.Constructions;
using TanksGame.Core.GameObjects;

namespace TanksGame.Core.GameProcess
{
    public class TankWorld : IWorld
    {
        private readonly Dictionary<UnitActions, int> _actions;
        
        private readonly bool _draw;

        private readonly MapState _startingMap;

        private readonly List<IUnit> _units;
        private readonly Dictionary<IUnit, int> _bullets;
        private readonly Dictionary<IUnit, int> _oil;
        private readonly Dictionary<IUnit, int> _result;

        private int _iter;

        private Dictionary<IUnit, bool> _turns;
        private List<IUnit> _shooters;
        private List<IUnit> _movers;
        private List<IUnit> _dead;
        private MapState CurrentMapState { get; set; }
        
        private MapState PrevMapState { get; set; }


        private void DrawWorld()
        {
            Console.Clear();
            Console.WriteLine(_iter);
            var map = CurrentMapState.Map;
            for (var y = GameRules.MapHeight - 1; y >= 0; y--)
            {
                for (var x = 0; x < GameRules.MapWidth; x++)
                {
                    var symb = " ";
                    symb = map[y][x].getTag() switch
                    {
                        "Oil" => "¤",
                        "Bullets" => "Ш",
                        "Wall" => "#",
                        "Space" => " ",
                        "Tank" => ((IUnit) map[y][x]).Direction switch
                        {
                            0 => "v",
                            1 => "<",
                            2 => "^",
                            3 => ">",
                            _ => symb
                        },
                        _ => symb
                    };
                    Console.Write(symb + " ");
                }
                Console.WriteLine();
            }
            Console.ReadLine();
            Console.Clear();
        }
        
        private bool Tick()
        {
            /*var me = CurrentMapState.GetCoordinates(_units[0]);
            Console.WriteLine(me.X + " " + me.Y + " " + _units[0].Direction);
            */
            _movers = new List<IUnit>();
            _shooters = new List<IUnit>();
            _dead = new List<IUnit>();
            _turns = new Dictionary<IUnit, bool>();
            PrevMapState = new MapState();
            if (_draw) DrawWorld();
            foreach (var unit in _units)
            {
                var act = unit.GetAction(GameRules.GetData(unit, CurrentMapState, _oil[unit], _bullets[unit]));
                AnalyzeAction(unit, act);
                PrevMapState.AddObject(unit, CurrentMapState.GetCoordinates(unit));
            }
            ApplyTurns();
            ApplyMovers();
            ApplyShooters();

            foreach (var unit in _units)
            {
                _oil[unit] -= 1;
                if (_oil[unit] < 1) _dead.Add(unit);
            }
            if(_units.Count <= 1)
                foreach (var unit in _units)
                {
                    _dead.Add(unit);
                    _iter += _oil[_units[0]];
                }
            foreach (var unit in _dead) KillUnit(unit);
            _iter++;
            //Console.WriteLine(iter);
            return _units.Count != 0;
        }

        private void KillUnit(IUnit unit)
        {
            CurrentMapState.RemoveObject(unit);
            _result[unit] = _iter;
            _units.Remove(unit);
        }

        private void ApplyShooters()
        {
            foreach (var unit in _shooters.Where(unit => _bullets[unit] > 0))
            {
                _bullets[unit] -= 1;
                Shoot(unit);
            }
        }

        private void Shoot(IUnit unit)
        {
            var coordinates = CurrentMapState.GetCoordinates(unit);
            var direction = unit.Direction;
            var bulletCoordinates = new Coordinates(coordinates);
            for (var i = 0; i < GameRules.ShootingRange; i++)
            {
                bulletCoordinates =
                    GameRules.CalculateRelativeCoordinates(
                        bulletCoordinates, 
                        new Coordinates(0, 1), 
                        direction);
                if(CurrentMapState.HasCoordinates(bulletCoordinates))
                    switch (CurrentMapState.GetObject(bulletCoordinates).getTag())
                    {
                        case "Tank":
                            var killedEnemy = (IUnit) CurrentMapState.GetObject(bulletCoordinates);
                            _dead.Add(killedEnemy);
                            _oil[unit] += _oil[killedEnemy];
                            return;
                        case "Wall":
                            return;
                    }
            }
        }

        private void ApplyMovers()
        {
            foreach (var unit in _movers)
                CurrentMapState.RemoveObject(unit);
            
            foreach (var unit in _movers)
            {
                var coord = PrevMapState.GetCoordinates(unit);
                var newCoord = GameRules
                    .CalculateRelativeCoordinates(coord, new Coordinates(0, 1), unit.Direction);

                //was at front of us?
                if (PrevMapState.HasCoordinates(newCoord))
                {
                    //stay
                    CurrentMapState.AddObject(unit, PrevMapState.GetCoordinates(unit));
                }
                else 
                //will be at front of us?
                if(CurrentMapState.HasCoordinates(newCoord))
                    switch (CurrentMapState.GetObject(newCoord).getTag())
                    {
                        case "Wall"://stay
                            CurrentMapState.AddObject(unit, PrevMapState.GetCoordinates(unit));
                            break;
                        case "Oil" ://go
                            CurrentMapState.AddObject(unit, newCoord);
                            _oil[unit] += GameRules.OilBonus;
                            if (_oil[unit] > GameRules.MaxOil)
                                _oil[unit] = GameRules.MaxOil;
                            break;
                        case "Bullets"://go
                            CurrentMapState.AddObject(unit, newCoord);
                            _bullets[unit] += GameRules.BulletsBonus;
                            break;
                        case "Tank"://stay
                            CurrentMapState.AddObject(unit, PrevMapState.GetCoordinates(unit));
                            break;
                    }
                else
                {
                    CurrentMapState.AddObject(unit, newCoord);
                }
                
            }
        }

        private void ApplyTurns()
        {
            foreach (var (unit, turnRight) in _turns)
            {
                unit.Direction = GameRules.GetRelativeDirection(turnRight ? 3 : 1, unit.Direction);
            }
        }
        
        private void AnalyzeAction(IUnit unit, UnitActions action)
        {
            switch (action)
            {
                case UnitActions.Shoot:
                    _shooters.Add(unit);
                    break;
                case UnitActions.Stay:
                    break;
                case UnitActions.MoveForward:
                    _movers.Add(unit);
                    break;
                case UnitActions.TurnRight:
                    _turns.Add(unit, true);
                    break;
                case UnitActions.TurnLeft:
                    _turns.Add(unit, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }

            _actions[action]++;
        }

        public Dictionary<Func<List<double>, int> , int> Evaluate()
        {
            _iter = 0;
            CurrentMapState = _startingMap;
            while (Tick()){ }
            var results = new Dictionary<Func<List<double>, int>, int>();
            foreach(var (unit, value) in _result)
                results.Add(((Tank)(unit)).Decision, value);
            return results;
        }
        

        private void InitializeUnits()
        {
            foreach (var unit in _units)
            {
                _bullets[unit] = GameRules.StartBullets;
                _oil[unit] = GameRules.StartOil;
                _result[unit] = 0;
            }
        }

        public TankWorld(List<Tank> units, MapState startingMap, bool draw = false)
        {
            _startingMap = startingMap;
            _draw = draw;
            _units = units.Select(t => (IUnit) (t)).ToList();
            _actions = new Dictionary<UnitActions, int>()
            {
                {UnitActions.Shoot, 0},
                {UnitActions.Stay, 0},
                {UnitActions.MoveForward, 0},
                {UnitActions.TurnLeft, 0},
                {UnitActions.TurnRight, 0},
            };
            _bullets = new Dictionary<IUnit, int>();
            _oil = new Dictionary<IUnit, int>();
            _result = new Dictionary<IUnit, int>();
            InitializeUnits();
        }
    }
}