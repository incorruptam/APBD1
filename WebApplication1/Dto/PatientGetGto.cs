using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Dto;

public class PatientGetGto
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DataType Birthdata { get; set; }
}
    

