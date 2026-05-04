namespace StatHammer.Server.Models.DTOs.Simulations
{
    public class TurnStatReadDto
    {

        public int Id { get; set; }
        public int TurnNumber { get; set; }

        public string Side { get; set; } = string.Empty;

        public double AvgModelsAlive { get; set; }

        public double AvgWoundsAlive { get; set; }

        public double AvgHits { get; set; }

        public double AvgWounds { get; set; }

        public double AvgSuccessfulSaves { get; set; }

        public double AvgBlockedByFnp { get; set; }

        public List<WeaponStatReadDto> WeaponStats { get; set; } = new();
    }
}
