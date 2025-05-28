using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models;

public class Prescription
{
    [Key]
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    
    public int IdPatient { get; set; }
    public virtual Patient Patient { get; set; }

    public int IdDoctor { get; set; }
    public virtual Doctor Doctor { get; set; }

    public virtual ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
}