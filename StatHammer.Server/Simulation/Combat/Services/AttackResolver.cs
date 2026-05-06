using StatHammer.Server.Simulation.Combat.Models;
using StatHammer.Server.Simulation.Dice.Services;
using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Simulation.Combat.Services
{
    public class AttackResolver : IAttackResolver
    {
        private readonly IDiceExpressionParser _diceExpressionParser;
        private readonly IDiceRoller _diceRoller;
        private readonly ICombatDiceService _combatDiceService;

        public AttackResolver(
            IDiceExpressionParser diceExpressionParser,
            IDiceRoller diceRoller,
            ICombatDiceService combatDiceService)
        {
            _diceExpressionParser = diceExpressionParser;
            _diceRoller = diceRoller;
            _combatDiceService = combatDiceService;
        }

        public AttackResolutionResult ResolveAttack(
            SimulationModel attacker,
            SimulationModel defender,
            SimulationWeapon weapon,
            SimulationWeaponProfile weaponProfile)
        {
            var attacks = RollExpressionTotal(weaponProfile.Attacks);
            var hits = _combatDiceService.CountSuccesses(attacks, weaponProfile.Skill);

            var woundTarget = GetWoundTarget(weaponProfile.Strength, defender.Toughness);
            var wounds = _combatDiceService.CountSuccesses(hits, woundTarget);

            var saveTarget = GetSaveTarget(defender, weaponProfile.ArmorPiercing);
            var successfulSaves = _combatDiceService.CountSuccesses(wounds, saveTarget);

            var unsavedWounds = Math.Max(0, wounds - successfulSaves);

            var damageBeforeFnp = 0;
            for (int i = 0; i < unsavedWounds; i++)
            {
                damageBeforeFnp += RollExpressionTotal(weaponProfile.Damage);
            }

            var blockedByFnp = 0;
            if (defender.FeelNoPain.HasValue)
            {
                blockedByFnp = _combatDiceService.CountSuccesses(
                    damageBeforeFnp,
                    defender.FeelNoPain.Value);
            }

            var finalDamage = Math.Max(0, damageBeforeFnp - blockedByFnp);

            return new AttackResolutionResult
            {
                WeaponName = weapon.Name,
                WeaponProfileName = weaponProfile.Name,
                Attacks = attacks,
                Hits = hits,
                Wounds = wounds,
                SuccessfulSaves = successfulSaves,
                DamageBeforeFnp = damageBeforeFnp,
                BlockedByFnp = blockedByFnp,
                FinalDamage = finalDamage
            };
        }

        private int RollExpressionTotal(string expression)
        {
            var parsed = _diceExpressionParser.Parse(expression);
            return _diceRoller.Roll(parsed).Total;
        }

        private static int GetWoundTarget(int strength, int toughness)
        {
            if (strength >= toughness * 2)
                return 2;      
            else if (strength > toughness)            
                return 3;
            else if (strength == toughness)
                return 4;   
            else if (strength * 2 <= toughness)            
                return 6;            

            return 5;
        }

        private static int GetSaveTarget(SimulationModel defender, int armorPiercing)
        {
            var modifiedSave = defender.Save - armorPiercing;

            int bestSave = modifiedSave;

            if (defender.InvulnerableSave.HasValue)            
                bestSave = Math.Min(bestSave, defender.InvulnerableSave.Value);
            

            if (bestSave < 2)            
                bestSave = 2;
            

            return bestSave;
        }
    }
}
