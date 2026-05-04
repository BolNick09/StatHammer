namespace StatHammer.Server.Models.DTOs.Simulations
{
    public class WeaponStatCreateDto
    {
        public int WeaponId { get; set; }

        public double AvgHits { get; set; }

        public double AvgWounds { get; set; }

        public double AvgDamage { get; set; }
    }
}
