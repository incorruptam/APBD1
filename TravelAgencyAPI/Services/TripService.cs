public class TripService {
    private readonly IConfiguration _configuration;

    public TripService(IConfiguration configuration) {
        _configuration = configuration;
    }

    public async Task<List<Trip>> GetTripsAsync() {
        var trips = new List<Trip>();

        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();

        var command = new SqlCommand(
            @"SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,
              STRING_AGG(c.Name, ', ') AS Countries
              FROM Trip t
              JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
              JOIN Country c ON c.IdCountry = ct.IdCountry
              GROUP BY t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople", connection);

        var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync()) {
            trips.Add(new Trip {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                DateFrom = reader.GetDateTime(3),
                DateTo = reader.GetDateTime(4),
                MaxPeople = reader.GetInt32(5),
                Countries = reader.GetString(6).Split(',').Select(c => c.Trim()).ToList()
            });
        }

        return trips;
    }
}