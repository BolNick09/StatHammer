using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.PageServices.Admin.Wargears
{
    public class WargearPageInput
    {
        [Required]
        [Display(Name = "Название варгира")]
        public string Name { get; set; } = string.Empty;
    }
}