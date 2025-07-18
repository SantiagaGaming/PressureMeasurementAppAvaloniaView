using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace PressureMeasurementAppAvaloniaView.Data.Dto
{
    public class CreateMeasurementRequest
    {
        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        public List<PressureDto> Pressures { get; set; }

        [Required]
        public LifestyleDto Lifestyle { get; set; }
    }
}
