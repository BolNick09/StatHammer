using StatHammer.Server.Simulation.Combat.Models;
using StatHammer.Server.Simulation.Dice.Services;
using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Simulation.Combat.Services
{
    public class AttackResolver : IAttackResolver
    {
        private readonly IDiceExpressionParser _diceExpressionParser;
        private readonly IDiceRoller _diceRoller;

        public AttackResolver(
            IDiceExpressionParser diceExpressionParser,
            IDiceRoller diceRoller,
            ICombatDiceService combatDiceService)
        {
            _diceExpressionParser = diceExpressionParser;
            _diceRoller = diceRoller;
        }

        public AttackResolutionResult ResolveAttack(
            SimulationModel attacker,
            SimulationModel defender,
            SimulationWeapon weapon,
            SimulationWeaponProfile weaponProfile,
            UnitCombatModifiers? attackerModifiers = null,
            UnitCombatModifiers? defenderModifiers = null)
        {
            attackerModifiers ??= new UnitCombatModifiers();
            defenderModifiers ??= new UnitCombatModifiers();

            var attacks = RollExpressionTotal(weaponProfile.Attacks);

            var hits = CountModifiedD6Successes(
                attacks,
                weaponProfile.Skill,
                attackerModifiers.HitModifier);

            var woundTarget = GetWoundTarget(weaponProfile.Strength, defender.Toughness);

            var wounds = CountModifiedD6Successes(
                hits,
                woundTarget,
                attackerModifiers.WoundModifier);

            var effectiveArmorPiercing =
                weaponProfile.ArmorPiercing + attackerModifiers.ArmorPiercingModifier;

            var saveTarget = GetSaveTarget(defender, effectiveArmorPiercing);

            var successfulSaves = CountModifiedD6Successes(
                wounds,
                saveTarget,
                defenderModifiers.SaveModifier);

            var unsavedWounds = Math.Max(0, wounds - successfulSaves);

            var damagePackets = new List<int>();
            var damageBeforeFnp = 0;
            var blockedByFnp = 0;

            for (int i = 0; i < unsavedWounds; i++)
            {
                var damageRoll = RollExpressionTotal(weaponProfile.Damage);
                damageBeforeFnp += damageRoll;

                var blockedForThisPacket = 0;
                if (defender.FeelNoPain.HasValue)
                {
                    blockedForThisPacket = CountModifiedD6Successes(
                        damageRoll,
                        defender.FeelNoPain.Value,
                        modifier: 0);
                }

                blockedByFnp += blockedForThisPacket;

                var finalPacketDamage = Math.Max(0, damageRoll - blockedForThisPacket);
                if (finalPacketDamage > 0)
                {
                    damagePackets.Add(finalPacketDamage);
                }
            }

            var finalDamage = damagePackets.Sum();

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
                FinalDamage = finalDamage,
                DamagePackets = damagePackets
            };
        }

        private int RollExpressionTotal(string expression)
        {
            var parsed = _diceExpressionParser.Parse(expression);
            return _diceRoller.Roll(parsed).Total;
        }

        private int CountModifiedD6Successes(
            int rollCount,
            int target,
            int modifier)
        {
            if (rollCount <= 0)
            {
                return 0;
            }

            var successes = 0;

            for (int i = 0; i < rollCount; i++)
            {
                var roll = _diceRoller.Roll("D6").Total;
                var modifiedRoll = roll + modifier;

                if (modifiedRoll >= target)
                {
                    successes++;
                }
            }

            return successes;
        }

        private static int GetWoundTarget(int strength, int toughness)
        {
            if (strength >= toughness * 2)
            {
                return 2;
            }

            if (strength > toughness)
            {
                return 3;
            }

            if (strength == toughness)
            {
                return 4;
            }

            if (strength * 2 <= toughness)
            {
                return 6;
            }

            return 5;
        }

        private static int GetSaveTarget(SimulationModel defender, int armorPiercing)
        {
            var modifiedSave = defender.Save - armorPiercing;

            int bestSave = modifiedSave;

            if (defender.InvulnerableSave.HasValue)
            {
                bestSave = Math.Min(bestSave, defender.InvulnerableSave.Value);
            }

            if (bestSave < 2)
            {
                bestSave = 2;
            }

            return bestSave;
        }
    }
}