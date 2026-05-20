using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.PageServices.Admin.Abilities
{
    public class AbilityPageInput
    {
        [Required]
        [Display(Name = "Название правила")]
        public string Name { get; set; } = string.Empty;
    }
}
