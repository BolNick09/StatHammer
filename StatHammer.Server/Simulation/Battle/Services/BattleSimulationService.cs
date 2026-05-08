using StatHammer.Server.Simulation.Battle.Models;
using StatHammer.Server.Simulation.Combat.Services;
using StatHammer.Server.Simulation.Models;

namespace StatHammer.Server.Simulation.Battle.Services
{
    public class BattleSimulationService : IBattleSimulationService
    {
        private readonly IUnitCombatResolver _unitCombatResolver;

        public BattleSimulationService(IUnitCombatResolver unitCombatResolver)
        {
            _unitCombatResolver = unitCombatResolver;
        }

        public BattleSimulationResult SimulateBattle(
            SimulationUnit unitA,
            SimulationUnit unitB,
            int maxTurns)
        {
            var result = new BattleSimulationResult
            {
                UnitAName = unitA.Name,
                UnitBName = unitB.Name,
                MaxTurns = maxTurns
            };

            for (int turn = 1; turn <= maxTurns; turn++)
            {
                var turnResult = new BattleTurnResult
                {
                    TurnNumber = turn
                };

                if (!unitA.IsDestroyed)
                {
                    turnResult.SideAAction = ExecuteRangedAction("A", unitA, unitB);

                    if (unitB.IsDestroyed)
                    {
                        result.Turns.Add(turnResult);
                        result.CompletedTurns = turn;
                        FillFinalState(result, unitA, unitB);
                        result.Outcome = "UnitA";
                        return result;
                    }
                }

                if (!unitB.IsDestroyed)
                {
                    turnResult.SideBAction = ExecuteRangedAction("B", unitB, unitA);

                    if (unitA.IsDestroyed)
                    {
                        result.Turns.Add(turnResult);
                        result.CompletedTurns = turn;
                        FillFinalState(result, unitA, unitB);
                        result.Outcome = "UnitB";
                        return result;
                    }
                }

                result.Turns.Add(turnResult);
            }

            result.CompletedTurns = maxTurns;
            FillFinalState(result, unitA, unitB);

            if (unitA.IsDestroyed && !unitB.IsDestroyed)
            {
                result.Outcome = "UnitB";
            }
            else if (unitB.IsDestroyed && !unitA.IsDestroyed)
            {
                result.Outcome = "UnitA";
            }
            else
            {
                result.Outcome = DetermineWinnerByRemainingState(unitA, unitB);
            }

            return result;
        }

        private BattleSideTurnResult ExecuteRangedAction(
            string side,
            SimulationUnit attacker,
            SimulationUnit defender)
        {
            var startingAliveModels = defender.AliveModelCount;
            var startingTotalWounds = defender.TotalCurrentWounds;

            var phaseResult = _unitCombatResolver.ResolveRangedPhase(attacker, defender);

            return new BattleSideTurnResult
            {
                Side = side,
                AttackingUnitName = attacker.Name,
                TargetUnitName = defender.Name,
                StartingTargetAliveModels = startingAliveModels,
                StartingTargetTotalWounds = startingTotalWounds,
                EndingTargetAliveModels = defender.AliveModelCount,
                EndingTargetTotalWounds = defender.TotalCurrentWounds,
                TotalAttacks = phaseResult.AttackResult.TotalAttacks,
                TotalHits = phaseResult.AttackResult.TotalHits,
                TotalWounds = phaseResult.AttackResult.TotalWounds,
                TotalSuccessfulSaves = phaseResult.AttackResult.TotalSuccessfulSaves,
                TotalDamageBeforeFnp = phaseResult.AttackResult.TotalDamageBeforeFnp,
                TotalBlockedByFnp = phaseResult.AttackResult.TotalBlockedByFnp,
                TotalFinalDamage = phaseResult.AttackResult.TotalFinalDamage,
                ModelsKilled = phaseResult.AllocationResult.ModelsKilled,
                WeaponResults = phaseResult.AttackResult.GroupedWeaponResults
            };
        }

        private static void FillFinalState(
            BattleSimulationResult result,
            SimulationUnit unitA,
            SimulationUnit unitB)
        {
            result.UnitADestroyed = unitA.IsDestroyed;
            result.UnitBDestroyed = unitB.IsDestroyed;

            result.UnitAFinalAliveModels = unitA.AliveModelCount;
            result.UnitAFinalTotalWounds = unitA.TotalCurrentWounds;

            result.UnitBFinalAliveModels = unitB.AliveModelCount;
            result.UnitBFinalTotalWounds = unitB.TotalCurrentWounds;
        }

        private static string DetermineWinnerByRemainingState(
            SimulationUnit unitA,
            SimulationUnit unitB)
        {
            if (unitA.TotalCurrentWounds > unitB.TotalCurrentWounds)
            {
                return "UnitA";
            }

            if (unitB.TotalCurrentWounds > unitA.TotalCurrentWounds)
            {
                return "UnitB";
            }

            if (unitA.AliveModelCount > unitB.AliveModelCount)
            {
                return "UnitA";
            }

            if (unitB.AliveModelCount > unitA.AliveModelCount)
            {
                return "UnitB";
            }

            return "Draw";
        }
    }
}
