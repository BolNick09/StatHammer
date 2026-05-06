using StatHammer.Server.Simulation.Dice.Models;

namespace StatHammer.Server.Simulation.Dice.Services
{
    public interface IDiceExpressionParser
    {
        bool TryParse(string input, out DiceExpression? expression);

        DiceExpression Parse(string input);
    }

}
