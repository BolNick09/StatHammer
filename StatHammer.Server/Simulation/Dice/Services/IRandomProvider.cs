namespace StatHammer.Server.Simulation.Dice.Services
{
    public interface IRandomProvider
    {
        int Next(int minValue, int maxValue);
    }
}
