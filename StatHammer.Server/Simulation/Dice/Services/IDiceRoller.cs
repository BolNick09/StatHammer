using StatHammer.Server.Simulation.Dice.Models;

namespace StatHammer.Server.Simulation.Dice.Services
{
    public interface IDiceRoller
    {
        DiceRollResult Roll(string expression);

        DiceRollResult Roll(DiceExpression expression);
    }

}
