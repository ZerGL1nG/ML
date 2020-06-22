namespace TanksGame.Core.GameObjects
{
    public class Wall : IObject
    {
        public string getTag() => "Wall";
        public IObject Clone() => new Wall();

        public Wall(){ }
    }
}