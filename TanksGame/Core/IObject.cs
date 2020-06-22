namespace TanksGame.Core
{
    public interface IObject
    {
        string getTag();
        IObject Clone();
    }
}