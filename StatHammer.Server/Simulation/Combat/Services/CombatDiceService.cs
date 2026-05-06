using StatHammer.Server.Simulation.Dice.Services;

namespace StatHammer.Server.Simulation.Combat.Services
{
    public class CombatDiceService : ICombatDiceService
    {
        private readonly IDiceRoller _diceRoller;

        public CombatDiceService(IDiceRoller diceRoller)
        {
            _diceRoller = diceRoller;
        }

        public int RollD6()
        {
            return _diceRoller.Roll("D6").Total;
        }

        public int RollManyD6(int count)
        {
            if (count <= 0)
            {
                return 0;
            }

            return _diceRoller.Roll($"{count}D6").Total;
        }

        public int CountSuccesses(int diceCount, int targetValue)
        {
            if (diceCount <= 0)
            {
                return 0;
            }

            if (targetValue <= 1)
            {
                return diceCount;
            }

            if (targetValue > 6)
            {
                return 0;
            }

            var successes = 0;

            for (int i = 0; i < diceCount; i++)
            {
                var roll = RollD6();
                if (roll >= targetValue)
                {
                    successes++;
                }
            }

            return successes;
        }

        public int CountSuccesses(IEnumerable<int> rolls, int targetValue)
        {
            if (targetValue <= 1)
            {
                return rolls.Count();
            }

            if (targetValue > 6)
            {
                return 0;
            }

            return rolls.Count(r => r >= targetValue);
        }
    }
}
