using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.PageServices.Admin.Keywords
{
    public class KeywordPageInput
    {
        [Required]
        [Display(Name = "Ключевое слово")]
        public string Name { get; set; } = string.Empty;
    }
}