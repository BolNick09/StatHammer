namespace StatHammer.Server.Simulation.Combat.DTOs
{
    public class ApplyUnitDamageRequestDto
    {
        public int DefenderUnitId { get; set; }

        public List<int> DamagePackets { get; set; } = new();

        public bool DefenderPrefersMelee { get; set; }
    }
}
