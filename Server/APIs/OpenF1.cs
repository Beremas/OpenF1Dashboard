using OpenF1Dashboard.Server.Models;
using OpenF1Dashboard.Shared.Models;

namespace OpenF1Dashboard.Server.DataSource
{
    public class OpenF1
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly HttpClient _http;
        private readonly string _baseApi;

        public OpenF1(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
            _http = _httpFactory.CreateClient();
            _baseApi = "https://api.openf1.org/v1/";
        }

        public async Task<List<Driver>> GetDriversAsync(int? session_key = null, int? meeting_key = null, int? driver_number = null)
        {
           await Task.Delay(250);
            var queryParams = new List<string>();

            if (driver_number.HasValue) queryParams.Add($"driver_number={driver_number.Value}");
            if (meeting_key.HasValue) queryParams.Add($"meeting_key={meeting_key.Value}");
            if (session_key.HasValue) queryParams.Add($"session_key={session_key.Value}");
           
            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
            var url = $"{_baseApi}drivers{queryString}";

            try
            {
                var response = await _http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Request to {url} failed with {(int)response.StatusCode} {response.ReasonPhrase}. Response: {content}");
                }

                return await response.Content.ReadFromJsonAsync<List<Driver>>() ?? new List<Driver>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET Request for {url} failed. Error: {ex}");
                throw;
            }
        }

        public async Task<List<Overtake>> GetOvertakesAsync(int? session_key = null, int? meeting_key = null)
        {
           await Task.Delay(250);
            var queryParams = new List<string>();

            if (meeting_key.HasValue) queryParams.Add($"meeting_key={meeting_key.Value}");
            if (session_key.HasValue) queryParams.Add($"session_key={session_key.Value}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
            var url = $"{_baseApi}overtakes{queryString}";

            try
            {
                var response = await _http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Request to {url} failed with {(int)response.StatusCode} {response.ReasonPhrase}. Response: {content}");
                }

                return await response.Content.ReadFromJsonAsync<List<Overtake>>() ?? new List<Overtake>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET Request for {url} failed. Error: {ex}");
                throw;
            }
        }

        public async Task<List<Session>> GetSessionsAsync(int? session_key = null, string? location = null, string? country_name = null, string? session_name = null, int? year = null, string? circuit_short_name = null)
        {
           await Task.Delay(250);
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(country_name)) queryParams.Add($"country_name={Uri.EscapeDataString(country_name)}");
            if (!string.IsNullOrWhiteSpace(session_name)) queryParams.Add($"session_name={Uri.EscapeDataString(session_name)}");
            if (!string.IsNullOrWhiteSpace(location)) queryParams.Add($"location={Uri.EscapeDataString(location)}");
            if (!string.IsNullOrWhiteSpace(circuit_short_name)) queryParams.Add($"circuit_short_name={Uri.EscapeDataString(circuit_short_name)}");
            if (session_key.HasValue) queryParams.Add($"session_key={session_key.Value}");
            if (year.HasValue) queryParams.Add($"year={year.Value}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
            var url = $"{_baseApi}sessions{queryString}";

            try
            {
                var response = await _http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Request to {url} failed with {(int)response.StatusCode} {response.ReasonPhrase}. Response: {content}");
                }

                return await response.Content.ReadFromJsonAsync<List<Session>>() ?? new List<Session>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET Request for {url} failed. Error: {ex}");
                throw;
            }
        }

        public async Task<List<SessionResult>> GetSessionResultAsync(int? session_key = null, int? driver_number = null, int? meeting_key = null, string? position = null)
        {
            await Task.Delay(250);
            var queryParams = new List<string>();

            if (driver_number.HasValue) queryParams.Add($"driver_number={driver_number.Value}");
            if (meeting_key.HasValue) queryParams.Add($"meeting_key={meeting_key.Value}");
            if (session_key.HasValue) queryParams.Add($"session_key={session_key.Value}");
            if (!string.IsNullOrWhiteSpace(position))
            {
                for (int pos = 1; pos <= 3; pos++)
                {
                    queryParams.Add($"position={pos}");
                }
            }

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
            var url = $"{_baseApi}session_result{queryString}";

            try
            {
                var response = await _http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Request to {url} failed with {(int)response.StatusCode} {response.ReasonPhrase}. Response: {content}");
                }

                return await response.Content.ReadFromJsonAsync<List<SessionResult>>() ?? new List<SessionResult>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET Request for {url} failed. Error: {ex}");
                throw;
            }

        }

        public async Task<List<Lap>> GetLapsAsync(int? session_key = null, int? driver_number = null, int? meeting_key = null, int? lap_number = null)
        {
           await Task.Delay(250);
            var queryParams = new List<string>();

            if (driver_number.HasValue) queryParams.Add($"driver_number={driver_number.Value}");
            if (meeting_key.HasValue) queryParams.Add($"meeting_key={meeting_key.Value}");
            if (session_key.HasValue) queryParams.Add($"session_key={session_key.Value}");
            if (lap_number.HasValue) queryParams.Add($"lap_number={lap_number.Value}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
            var url = $"{_baseApi}laps{queryString}";

            try
            {
                var response = await _http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Request to {url} failed with {(int)response.StatusCode} {response.ReasonPhrase}. Response: {content}");
                }

                return await response.Content.ReadFromJsonAsync<List<Lap>>() ?? new List<Lap>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET Request for {url} failed. Error: {ex}");
                throw;
            }
        }

        public async Task<List<Pit>> GetPitsAsync(int? session_key = null, int? driver_number = null, int? meeting_key = null)
        {
           await Task.Delay(250);
            var queryParams = new List<string>();

            if (driver_number.HasValue) queryParams.Add($"driver_number={driver_number.Value}");
            if (meeting_key.HasValue) queryParams.Add($"meeting_key={meeting_key.Value}");
            if (session_key.HasValue) queryParams.Add($"session_key={session_key.Value}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
            var url = $"{_baseApi}pit{queryString}";

            try
            {
                var response = await _http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Request to {url} failed with {(int)response.StatusCode} {response.ReasonPhrase}. Response: {content}");
                }

                return await response.Content.ReadFromJsonAsync<List<Pit>>() ?? new List<Pit>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET Request for {url} failed. Error: {ex}");
                throw;
            }
        }

        public async Task<List<Stint>> GetStintsAsync(int? session_key = null, int? driver_number = null, int? meeting_key = null, string? tyre_age_at_start = null, string? pit_duration = null)
        {
           await Task.Delay(250);
            var queryParams = new List<string>();

            if (driver_number.HasValue) queryParams.Add($"driver_number={driver_number.Value}");
            if (meeting_key.HasValue) queryParams.Add($"meeting_key={meeting_key.Value}");
            if (session_key.HasValue) queryParams.Add($"session_key={session_key.Value}");
            if (!string.IsNullOrWhiteSpace(tyre_age_at_start)) queryParams.Add($"tyre_age_at_start={Uri.EscapeDataString(tyre_age_at_start)}");
            if (!string.IsNullOrWhiteSpace(pit_duration)) queryParams.Add($"pit_duration={Uri.EscapeDataString(pit_duration)}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
            var url = $"{_baseApi}stints{queryString}";

            try
            {
                var response = await _http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Request to {url} failed with {(int)response.StatusCode} {response.ReasonPhrase}. Response: {content}");
                }

                return await response.Content.ReadFromJsonAsync<List<Stint>>() ?? new List<Stint>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET Request for {url} failed. Error: {ex}");
                throw;
            }
        }
    }
}
