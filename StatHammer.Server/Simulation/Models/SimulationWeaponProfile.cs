namespace StatHammer.Server.Simulation.Models
{
    public class SimulationWeaponProfile
    {
        public int WeaponProfileId { get; set; }

        public string? Name { get; set; }

        public int Range { get; set; }

        public string Attacks { get; set; } = string.Empty;

        public int Skill { get; set; }

        public int Strength { get; set; }

        public int ArmorPiercing { get; set; }

        public string Damage { get; set; } = string.Empty;

        public List<string> Abilities { get; set; } = new();

        public bool IsMelee => Range == 0;
    }

}
