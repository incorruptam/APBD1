
namespace WebApplication1.Dto;

public class PatientDto
{
    public int? IdPatient { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Email { get; set; }
    public ICollection<int> Doctor { get; set; }
}