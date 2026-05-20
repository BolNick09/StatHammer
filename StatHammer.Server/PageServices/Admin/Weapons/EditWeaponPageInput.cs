using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.PageServices.Admin.Weapons
{
    public class EditWeaponPageInput
    {
        [Required]
        [Display(Name = "Название оружия")]
        public string Name { get; set; } = string.Empty;

        public List<EditWeaponProfilePageInput> Profiles { get; set; } = new();
    }
}