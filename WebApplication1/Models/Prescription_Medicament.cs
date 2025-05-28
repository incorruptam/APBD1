using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models;
[Table("Prescription_Medicament")]
[PrimaryKey(nameof(IdMedicament),nameof(IdPrescription))]
public class PrescriptionMedicament
{
    [Column("IdMedicament")]
    public int IdMedicament { get; set; }
    [ForeignKey(nameof(IdMedicament))]
    public virtual Medicament Medicament { get; set; } = null!;
    [Column("IdPrescription")]
    public int IdPrescription { get; set; }
    [ForeignKey(nameof(IdPrescription))]
    public virtual Prescription Prescription { get; set; } = null!;
    public int Dose { get; set; }
    [MaxLength(100)]
    public string Details { get; set; } = null!;
}