namespace StatHammer.Server.Models.Entities
{
    public class WeaponStat
    {
        public int Id { get; set; }

        public int TurnStatId { get; set; }

        public int WeaponId { get; set; }

        public double AvgHits { get; set; }

        public double AvgWounds { get; set; }

        public double AvgDamage { get; set; }

        public Weapon? Weapon { get; set; }

        public TurnStat? TurnStat { get; set; }
    }
}
