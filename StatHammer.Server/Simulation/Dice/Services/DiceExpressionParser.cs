using StatHammer.Server.Simulation.Dice.Models;
using System.Text.RegularExpressions;

namespace StatHammer.Server.Simulation.Dice.Services
{
    public class DiceExpressionParser : IDiceExpressionParser
    {
        private static readonly Regex FlatValueRegex =
            new(@"^\d+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex DiceRegex =
            new(@"^(?:(\d*)D(\d+))(?:([+-]\d+))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool TryParse(string input, out DiceExpression? expression)
        {
            expression = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var normalized = input.Trim().Replace(" ", "").ToUpperInvariant();

            if (FlatValueRegex.IsMatch(normalized))
            {
                expression = new DiceExpression
                {
                    DiceCount = 0,
                    DiceSides = 0,
                    Modifier = int.Parse(normalized)
                };

                return true;
            }

            var match = DiceRegex.Match(normalized);
            if (!match.Success)
            {
                return false;
            }

            var diceCountRaw = match.Groups[1].Value;
            var diceSidesRaw = match.Groups[2].Value;
            var modifierRaw = match.Groups[3].Value;

            var diceCount = string.IsNullOrEmpty(diceCountRaw) ? 1 : int.Parse(diceCountRaw);
            var diceSides = int.Parse(diceSidesRaw);
            var modifier = string.IsNullOrEmpty(modifierRaw) ? 0 : int.Parse(modifierRaw);

            if (diceCount < 1 || diceSides < 1)
            {
                return false;
            }

            expression = new DiceExpression
            {
                DiceCount = diceCount,
                DiceSides = diceSides,
                Modifier = modifier
            };

            return true;
        }

        public DiceExpression Parse(string input)
        {
            if (!TryParse(input, out var expression) || expression == null)
            {
                throw new FormatException($"Invalid dice expression: '{input}'.");
            }

            return expression;
        }
    }

}
