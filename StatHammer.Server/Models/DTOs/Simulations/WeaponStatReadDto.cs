namespace StatHammer.Server.Models.DTOs.Simulations
{
    public class WeaponStatReadDto
    {
        public int Id { get; set; }

        public int WeaponId { get; set; }

        public string WeaponName { get; set; } = string.Empty;

        public double AvgHits { get; set; }

        public double AvgWounds { get; set; }

        public double AvgDamage { get; set; }
    }
}
