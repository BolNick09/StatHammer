using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.PageServices.Admin.Weapons
{
    public class CreateWeaponPageInput
    {
        [Required]
        [Display(Name = "Название оружия")]
        public string Name { get; set; } = string.Empty;
        public bool HasSecondProfile { get; set; }

        public List<CreateWeaponProfilePageInput> Profiles { get; set; } = new()
    {
        new CreateWeaponProfilePageInput(),
        new CreateWeaponProfilePageInput()
    };
    }
}
