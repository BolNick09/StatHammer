namespace StatHammer.Server.PageServices.Admin.SimulationResults
{
    public class SimulationResultWeaponStatViewModel
    {
        public int Id { get; set; }

        public int WeaponId { get; set; }

        public string WeaponName { get; set; } = string.Empty;

        public double AvgHits { get; set; }

        public double AvgWounds { get; set; }

        public double AvgDamage { get; set; }
    }
}