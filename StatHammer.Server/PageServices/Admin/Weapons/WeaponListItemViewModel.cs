namespace StatHammer.Server.PageServices.Admin.Weapons
{
    public class WeaponListItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<WeaponProfileListItemViewModel> Profiles { get; set; } = new();
    }
}
