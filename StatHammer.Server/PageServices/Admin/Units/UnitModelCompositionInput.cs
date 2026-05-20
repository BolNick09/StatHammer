using System.ComponentModel.DataAnnotations;

namespace StatHammer.Server.PageServices.Admin.Units
{
    public class UnitModelCompositionInput
    {
        [Display(Name = "Модель")]
        public int? ModelId { get; set; }

        [Range(0, 30)]
        [Display(Name = "Минимум")]
        public int MinCount { get; set; }

        [Range(0, 30)]
        [Display(Name = "Максимум")]
        public int MaxCount { get; set; }
    }
}