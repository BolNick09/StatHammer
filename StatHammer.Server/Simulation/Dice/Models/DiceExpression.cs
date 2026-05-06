namespace StatHammer.Server.Simulation.Dice.Models
{
    public class DiceExpression
    {
        public int DiceCount { get; set; }

        public int DiceSides { get; set; }

        public int Modifier { get; set; }

        public bool IsFlatValue => DiceCount == 0;

        public int FlatValue => Modifier;

        public override string ToString()
        {
            if (IsFlatValue)
            {
                return Modifier.ToString();
            }

            var dicePart = DiceCount == 1
                ? $"D{DiceSides}"
                : $"{DiceCount}D{DiceSides}";

            if (Modifier == 0)
            {
                return dicePart;
            }

            return Modifier > 0
                ? $"{dicePart}+{Modifier}"
                : $"{dicePart}{Modifier}";
        }
    }

}
