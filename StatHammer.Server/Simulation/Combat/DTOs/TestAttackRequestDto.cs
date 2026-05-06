namespace StatHammer.Server.Simulation.Combat.DTOs
{
    public class TestAttackRequestDto
    {
        public string Attacks { get; set; } = "1";

        public int Skill { get; set; }

        public int Strength { get; set; }

        public int ArmorPiercing { get; set; }

        public string Damage { get; set; } = "1";

        public int DefenderToughness { get; set; }

        public int DefenderSave { get; set; }

        public int? DefenderInvulnerableSave { get; set; }

        public int? DefenderFeelNoPain { get; set; }
    }
}
