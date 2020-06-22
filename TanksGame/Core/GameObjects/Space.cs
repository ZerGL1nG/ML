namespace TanksGame.Core.GameObjects
{
    public class Space : IObject
    {
        public string getTag() => "Space";
        public IObject Clone() => new Space();
        public Space(){ }
    }
}