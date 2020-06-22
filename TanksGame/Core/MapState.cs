using System;
using System.Collections;
using System.Collections.Generic;
using TanksGame.Core.Constructions;
using TanksGame.Core.GameObjects;

namespace TanksGame.Core
{
    public class MapState
    {
        public readonly List<List<IObject>> Map;
        private readonly Dictionary<IObject, Coordinates> _objects;
        public Coordinates GetCoordinates(IObject obj) => _objects.ContainsKey(obj) ? _objects[obj] : new Coordinates(0, 0);
        public IObject GetObject(Coordinates coord) => Map[coord.Y][coord.X];
        public bool HasCoordinates(Coordinates coord) => Map[coord.Y][coord.X].getTag()!="Space";
        public bool HasObject(IObject obj) => _objects.ContainsKey(obj);
        public IEnumerable<IObject> GetObjects() => _objects.Keys;
        
        public void AddObject(IObject obj, Coordinates coord)
        {
            var prevObj = Map[coord.Y][coord.X];
            if(prevObj.getTag() != "Space")
                _objects.Remove(prevObj);
            _objects.Add(obj, coord);
            Map[coord.Y][coord.X] = obj;
        }

        public void RemoveObject(IObject obj)
        {
            if (!_objects.ContainsKey(obj)) return;
            var newObj = new Space();
            var coord = _objects[obj];
            Map[coord.Y][coord.X] = newObj;
            _objects.Remove(obj);
        }

        public void RemoveCoordinates(Coordinates coord)
        {
            var newObj = new Space();
            _objects.Remove(Map[coord.Y][coord.X]);
            Map[coord.Y][coord.X] = newObj;
        }


        public MapState(MapState state)
        {
            _objects = new Dictionary<IObject, Coordinates>(state._objects);
            Map = new List<List<IObject>>();
            for (var y = 0; y < GameRules.MapHeight; y++)
            {
                Map.Add(new List<IObject>());
                for (var x = 0; x < GameRules.MapWidth; x++)
                    Map[y].Add(state.Map[y][x].Clone());
            }
        }
        
        public MapState()
        {
            _objects = new Dictionary<IObject, Coordinates>();
            Map = new List<List<IObject>>();
            for (var y = 0; y < GameRules.MapHeight; y++)
            {
                Map.Add(new List<IObject>());
                for (var x = 0; x < GameRules.MapWidth; x++)
                    Map[y].Add(new Space());
            }
        }
    }
}