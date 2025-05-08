using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TravelAgencyAPI.Models;

namespace TravelAgencyAPI.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ClientsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult CreateClient([FromBody] Client client)
        {
            if (string.IsNullOrEmpty(client.FirstName) || string.IsNullOrEmpty(client.Pesel))
                return BadRequest("Missing required fields.");

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand(@"
                INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
                OUTPUT INSERTED.IdClient
                VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)", connection);

            command.Parameters.AddWithValue("@FirstName", client.FirstName);
            command.Parameters.AddWithValue("@LastName", client.LastName);
            command.Parameters.AddWithValue("@Email", client.Email);
            command.Parameters.AddWithValue("@Telephone", client.Telephone);
            command.Parameters.AddWithValue("@Pesel", client.Pesel);

            connection.Open();
            int newId = (int)command.ExecuteScalar();
            return Created($"api/clients/{newId}", new { Id = newId });
        }

        [HttpPut("{id}/trips/{tripId}")]
        public IActionResult RegisterClientToTrip(int id, int tripId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            connection.Open();

            var checkClientCmd = new SqlCommand("SELECT COUNT(1) FROM Client WHERE IdClient = @id", connection);
            checkClientCmd.Parameters.AddWithValue("@id", id);
            if ((int)checkClientCmd.ExecuteScalar() == 0)
                return NotFound("Client not found.");

            var checkTripCmd = new SqlCommand("SELECT COUNT(1) FROM Trip WHERE IdTrip = @tripId", connection);
            checkTripCmd.Parameters.AddWithValue("@tripId", tripId);
            if ((int)checkTripCmd.ExecuteScalar() == 0)
                return NotFound("Trip not found.");

            var insertCmd = new SqlCommand(@"
                INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt)
                VALUES (@id, @tripId, @registeredAt)", connection);

            insertCmd.Parameters.AddWithValue("@id", id);
            insertCmd.Parameters.AddWithValue("@tripId", tripId);
            insertCmd.Parameters.AddWithValue("@registeredAt", DateTime.UtcNow);

            insertCmd.ExecuteNonQuery();
            return Ok("Client registered to trip.");
        }

        [HttpDelete("{id}/trips/{tripId}")]
        public IActionResult DeleteClientTrip(int id, int tripId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            connection.Open();

            var checkCmd = new SqlCommand(@"
                SELECT COUNT(1) FROM Client_Trip WHERE IdClient = @id AND IdTrip = @tripId", connection);
            checkCmd.Parameters.AddWithValue("@id", id);
            checkCmd.Parameters.AddWithValue("@tripId", tripId);

            if ((int)checkCmd.ExecuteScalar() == 0)
                return NotFound("Registration not found.");

            var deleteCmd = new SqlCommand(@"
                DELETE FROM Client_Trip WHERE IdClient = @id AND IdTrip = @tripId", connection);

            deleteCmd.Parameters.AddWithValue("@id", id);
            deleteCmd.Parameters.AddWithValue("@tripId", tripId);
            deleteCmd.ExecuteNonQuery();

            return Ok("Registration deleted.");
        }

        [HttpGet("{id}/trips")]
        public IActionResult GetClientTrips(int id)
        {
            var trips = new List<Trip>();
            var tripDict = new Dictionary<int, Trip>();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand(@"
                SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name AS CountryName
                FROM Trip t
                JOIN Client_Trip ct ON t.IdTrip = ct.IdTrip
                JOIN Country_Trip ctr ON t.IdTrip = ctr.IdTrip
                JOIN Country c ON ctr.IdCountry = c.IdCountry
                WHERE ct.IdClient = @id", connection);

            command.Parameters.AddWithValue("@id", id);

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
