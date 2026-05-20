using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.PageServices.Admin.Models
{
    public class ModelPageInput
    {
        [Required]
        [Display(Name = "Название модели")]
        public string Name { get; set; } = string.Empty;

        [Range(0, 30)]
        [Display(Name = "Move")]
        public int Move { get; set; } = 6;

        [Range(1, 30)]
        [Display(Name = "Toughness")]
        public int Toughness { get; set; } = 3;

        [Range(2, 7)]
        [Display(Name = "Save")]
        public int Save { get; set; } = 4;

        [Range(2, 7)]
        [Display(Name = "Invulnerable Save")]
        public int? InvulnerableSave { get; set; }

        [Range(1, 100)]
        [Display(Name = "Wounds")]
        public int Wounds { get; set; } = 1;

        [Range(2, 10)]
        [Display(Name = "Leadership")]
        public int Leadership { get; set; } = 7;

        [Range(0, 50)]
        [Display(Name = "OC")]
        public int OC { get; set; } = 1;

        [Display(Name = "Оружие")]
        public List<int>? WeaponIds { get; set; }

        [Display(Name = "Варгир")]
        public List<int>? WargearIds { get; set; }

    }
}