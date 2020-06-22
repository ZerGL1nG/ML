namespace TanksGame.Core.GameObjects
{
    public class Oil : IObject
    {
        public string getTag() => "Oil";
        public IObject Clone() => new Oil();

        public Oil() { }
    }
}