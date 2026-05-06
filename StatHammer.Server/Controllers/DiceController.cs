using Microsoft.AspNetCore.Mvc;
using StatHammer.Server.Simulation.Dice.Services;

namespace StatHammer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiceController : ControllerBase
    {
        private readonly IDiceExpressionParser _parser;
        private readonly IDiceRoller _roller;

        public DiceController(IDiceExpressionParser parser, IDiceRoller roller)
        {
            _parser = parser;
            _roller = roller;
        }

        [HttpGet("parse")]
        public IActionResult Parse([FromQuery] string expression)
        {
            var success = _parser.TryParse(expression, out var parsed);

            if (!success || parsed == null)
            {
                return BadRequest($"Invalid dice expression: {expression}");
            }

            return Ok(parsed);
        }

        [HttpGet("roll")]
        public IActionResult Roll([FromQuery] string expression)
        {
            try
            {
                var result = _roller.Roll(expression);
                return Ok(result);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
