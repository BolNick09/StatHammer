namespace StatHammer.Server.Simulation.Combat.DTOs
{
    public class ApplyUnitDamageRequestDto
    {
        public int DefenderUnitId { get; set; }

        public int Damage { get; set; }

        public bool DefenderPrefersMelee { get; set; }
    }
}
