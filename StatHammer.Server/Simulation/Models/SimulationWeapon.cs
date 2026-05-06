namespace StatHammer.Server.Simulation.Models
{
    public class SimulationWeapon
    {
        public int WeaponId { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<SimulationWeaponProfile> Profiles { get; set; } = new();

    }
}
