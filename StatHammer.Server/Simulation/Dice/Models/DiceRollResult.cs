namespace StatHammer.Server.Simulation.Dice.Models
{
    public class DiceRollResult
    {
        public string Expression { get; set; } = string.Empty;

        public List<int> Rolls { get; set; } = new();

        public int Modifier { get; set; }

        public int Total => Rolls.Sum() + Modifier;
    }
}
