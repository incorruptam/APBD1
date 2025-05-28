using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using WebApplication1.data;
using WebApplication1.Dto;
using WebApplication1.Exceptions;
using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IDbService
{
    public Task<ICollection<PatientGetGto>> GetPatientDetailsAsync();
}

public class DbService(AppDbContext data) : IDbService
{
    public async Task<ICollection<PatientGetGto>> GetPatientDetailsAsync()
    {
        return await data.Patients.Select(p => new PatientGetGto
        {
            IdPatient = p.IdPatient,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Birthdata = p.Birthdata,
        }).ToListAsync();
    }
}