namespace TanksGame.Core.GameObjects
{
    public class Bullets : IObject
    {
        public string getTag() => "Bullets";
        public IObject Clone() => new Bullets();

        public Bullets(){}
    }
}