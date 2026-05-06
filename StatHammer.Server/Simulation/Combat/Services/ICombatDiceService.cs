namespace StatHammer.Server.Simulation.Combat.Services
{
    public interface ICombatDiceService
    {
        int RollD6();

        int RollManyD6(int count);

        int CountSuccesses(int diceCount, int targetValue);

        int CountSuccesses(IEnumerable<int> rolls, int targetValue);
    }
}
