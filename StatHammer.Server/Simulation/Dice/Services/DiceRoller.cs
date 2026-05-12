using StatHammer.Server.Simulation.Dice.Models;

namespace StatHammer.Server.Simulation.Dice.Services
{
    public class DiceRoller : IDiceRoller
    {
        private readonly IDiceExpressionParser _parser;
        private readonly IRandomProvider _randomProvider;

        public DiceRoller(
            IDiceExpressionParser parser,
            IRandomProvider randomProvider)
        {
            _parser = parser;
            _randomProvider = randomProvider;
        }

        public DiceRollResult Roll(string expression)
        {
            var parsed = _parser.Parse(expression);
            return Roll(parsed);
        }

        public DiceRollResult Roll(DiceExpression expression)
        {
            var result = new DiceRollResult
            {
                Expression = expression.ToString(),
                Modifier = expression.Modifier
            };

            if (expression.IsFlatValue)
            {
                return result;
            }

            for (int i = 0; i < expression.DiceCount; i++)
            {
                result.Rolls.Add(_randomProvider.Next(1, expression.DiceSides + 1));
            }

            return result;
        }
    }

}
