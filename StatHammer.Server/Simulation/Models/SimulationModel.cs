namespace StatHammer.Server.Simulation.Models
{
    public class SimulationModel
    {
        public int ModelId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Move { get; set; }

        public int Toughness { get; set; }

        public int Save { get; set; }

        public int? InvulnerableSave { get; set; }

        public int MaxWounds { get; set; }

        public int CurrentWounds { get; set; }

        public int Leadership { get; set; }

        public int OC { get; set; }

        public List<SimulationWeapon> Weapons { get; set; } = new();

        public List<string> Abilities { get; set; } = new();

        public bool IsAlive => CurrentWounds > 0;
    }

}
