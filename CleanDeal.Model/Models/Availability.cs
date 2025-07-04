using System.ComponentModel.DataAnnotations;

namespace CleanDeal.Model.Models;

public class Availability
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Data jest wymagana")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; }

    [Required]
    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
    public TimeSpan StartTime { get; set; }

    [Required]
    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
    public TimeSpan EndTime { get; set; }

    public string CleanerId { get; set; } = null!;
    public ApplicationUser Cleaner { get; set; } = null!;
}