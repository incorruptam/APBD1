using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TravelAgencyAPI.Models;

namespace TravelAgencyAPI.Controllers
{
    [ApiController]
    [Route("api/trips")]
    public class TripsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TripsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetTrips()
        {
            var trips = new List<Trip>();
            var tripDict = new Dictionary<int, Trip>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand(@"
                SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,
                       c.Name AS CountryName
                FROM Trip t
                JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
                JOIN Country c ON ct.IdCountry = c.IdCountry
            ", connection);

            connection.Open();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int idTrip = reader.GetInt32(0);
                if (!tripDict.ContainsKey(idTrip))
                {
                    tripDict[idTrip] = new Trip
                    {
                        IdTrip = idTrip,
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        DateFrom = reader.GetDateTime(3),
                        DateTo = reader.GetDateTime(4),
                        MaxPeople = reader.GetInt32(5),
                        Countries = new List<string>()
                    };
                }
                tripDict[idTrip].Countries.Add(reader.GetString(6));
            }

            trips.AddRange(tripDict.Values);
            return Ok(trips);
        }
    }
}