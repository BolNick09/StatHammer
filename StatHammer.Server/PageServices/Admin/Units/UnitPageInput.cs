using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.PageServices.Admin.Units
{
    public class UnitPageInput
    {
        [Required]
        [Display(Name = "Название юнита")]
        public string Name { get; set; } = string.Empty;

        public List<UnitModelCompositionInput> Models { get; set; } = new();

        [Display(Name = "Ключевые слова")]
        public List<int>? KeywordIds { get; set; }
    }
}