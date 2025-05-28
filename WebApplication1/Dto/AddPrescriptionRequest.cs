using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Dto;

public class AddPrescriptionRequest
{
    public PatientDto Patient { get; set; }
    public int DoctorId { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public List<MedicamentDto> Medicaments { get; set; }
}