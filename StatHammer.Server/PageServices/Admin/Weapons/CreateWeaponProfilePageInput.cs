using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.PageServices.Admin.Weapons
{
    public class CreateWeaponProfilePageInput
    {
        [Display(Name = "Название профиля")]
        public string? Name { get; set; }

        [Range(0, 120)]
        [Display(Name = "Дальность")]
        public int Range { get; set; }

        [Required]
        [Display(Name = "Атаки")]
        public string Attacks { get; set; } = "1";

        [Range(2, 7)]
        [Display(Name = "Навык")]
        public int Skill { get; set; } = 4;

        [Range(1, 30)]
        [Display(Name = "Сила")]
        public int Strength { get; set; } = 4;

        [Range(0, 10)]
        [Display(Name = "AP")]
        public int ArmorPiercing { get; set; }

        [Required]
        [Display(Name = "Урон")]
        public string Damage { get; set; } = "1";
    }
}
