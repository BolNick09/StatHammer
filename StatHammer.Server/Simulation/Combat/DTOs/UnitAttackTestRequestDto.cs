namespace StatHammer.Server.Simulation.Combat.DTOs
{
    public class UnitAttackTestRequestDto
    {
        public int AttackerUnitId { get; set; }

        public int DefenderUnitId { get; set; }

        public bool AttackerPrefersMelee { get; set; }

        public bool DefenderPrefersMelee { get; set; }
    }
}
