#nullable enable
using System.Runtime.Intrinsics.X86;

namespace TanksGame.Core.Constructions
{
    public class Coordinates
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Coordinates(int x, int y, bool impossible = false)
        {
            X = x;
            Y = y;
            if(!impossible)Normalize();
        }

        public Coordinates(Coordinates coordinates)
        {
            this.X = coordinates.X;
            this.Y = coordinates.Y;
        }

        private void Normalize()
        {
            X %= GameRules.MapWidth;
            if (X < 0) X += GameRules.MapWidth;
            /*Y %= GameRules.MapHeight;
            if (Y < 0) Y += GameRules.MapHeight;
            */
            while (Y >= GameRules.MapHeight)
            {
                Y -= GameRules.MapHeight;
                X = GameRules.MapWidth - X - 1;
            }
            while (Y < 0)
            {
                Y += GameRules.MapHeight;
                X = GameRules.MapWidth - X - 1;
            }
        }


        public override bool Equals(object? obj) => 
            ((obj as Coordinates)!).X == this.X &&
            ((obj as Coordinates)!).Y == this.Y;

        public override int GetHashCode() => X + Y * 148148291;
    }
}