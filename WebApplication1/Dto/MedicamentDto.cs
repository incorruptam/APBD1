using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Dto;

public class MedicamentDto
{
    public int IdMedicament { get; set; }
    public int Dose { get; set; }
    [MaxLength(100)] 
    [Required]
    public string Description { get; set; }
}