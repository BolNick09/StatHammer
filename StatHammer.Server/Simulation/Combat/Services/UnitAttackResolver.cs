using StatHammer.Server.Simulation.Combat.Models;
using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Simulation.Combat.Services
{
    public class UnitAttackResolver : IUnitAttackResolver
    {
        private readonly IAttackResolver _attackResolver;

        public UnitAttackResolver(IAttackResolver attackResolver)
        {
            _attackResolver = attackResolver;
        }

        public UnitAttackResult ResolveRangedAttack(
            SimulationUnit attacker,
            SimulationUnit defender)
        {
            var result = new UnitAttackResult
            {
                AttackingUnitName = attacker.Name,
                DefendingUnitName = defender.Name
            };

            var primaryDefender = defender.Models.FirstOrDefault(m => m.IsAlive);
            if (primaryDefender == null)            
                return result;


            foreach (var attackingModel in attacker.Models.Where(m => m.IsAlive))
            {
                foreach (var weapon in attackingModel.Weapons)
                {
                    var selectedProfile = SelectBestRangedProfile(weapon);

                    if (selectedProfile == null)                    
                        continue;
                    

                    var weaponResult = _attackResolver.ResolveAttack(
                        attackingModel,
                        primaryDefender,
                        weapon,
                        selectedProfile);

                    result.WeaponResults.Add(weaponResult);

                    result.TotalAttacks += weaponResult.Attacks;
                    result.TotalHits += weaponResult.Hits;
                    result.TotalWounds += weaponResult.Wounds;
                    result.TotalSuccessfulSaves += weaponResult.SuccessfulSaves;
                    result.TotalDamageBeforeFnp += weaponResult.DamageBeforeFnp;
                    result.TotalBlockedByFnp += weaponResult.BlockedByFnp;
                    result.TotalFinalDamage += weaponResult.FinalDamage;
                }
            }

            result.GroupedWeaponResults = result.WeaponResults
                .GroupBy(wr => new { wr.WeaponName, wr.WeaponProfileName })
                .Select(group => new GroupedWeaponAttackResult
                {
                    WeaponName = group.Key.WeaponName,
                    WeaponProfileName = group.Key.WeaponProfileName,
                    Count = group.Count(),
                    TotalAttacks = group.Sum(x => x.Attacks),
                    TotalHits = group.Sum(x => x.Hits),
                    TotalWounds = group.Sum(x => x.Wounds),
                    TotalSuccessfulSaves = group.Sum(x => x.SuccessfulSaves),
                    TotalDamageBeforeFnp = group.Sum(x => x.DamageBeforeFnp),
                    TotalBlockedByFnp = group.Sum(x => x.BlockedByFnp),
                    TotalFinalDamage = group.Sum(x => x.FinalDamage)
                })
                .OrderBy(x => x.WeaponName)
                .ThenBy(x => x.WeaponProfileName)
                .ToList();

            return result;
        }

        private static SimulationWeaponProfile? SelectBestRangedProfile(SimulationWeapon weapon)
        {
            var rangedProfiles = weapon.Profiles
                .Where(p => !p.IsMelee)
                .ToList();

            if (!rangedProfiles.Any())
            {
                return null;
            }

            if (rangedProfiles.Count == 1)
            {
                return rangedProfiles[0];
            }

            return rangedProfiles
                .OrderByDescending(p => p.Strength)
                .ThenByDescending(p => p.ArmorPiercing)
                .ThenByDescending(p => p.Range)
                .First();
        }
    }
}
