using OpenF1Dashboard.API.Auth;
using OpenF1Dashboard.CustomConverter;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace OpenF1Dashboard.API
{
	public class HttpApiClient 
	{

		private AuthenticationController _authenticationController;
		private readonly HttpClient _httpClient;
		public readonly Dictionary<string, string>? endpoints;

		public HttpApiClient ()
		{
			_authenticationController = new();
			_httpClient = new HttpClient();
			_httpClient.DefaultRequestHeaders.Accept.Clear();
			_httpClient.DefaultRequestVersion = HttpVersion.Version20;
			_httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			endpoints = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("Endpoints.json"));
		}


		public async Task<T?> GetAsync<T>(string? apiUrl, string? queryString = null, bool liveFeed = false, int? delayMs = null)
		{
			if (string.IsNullOrWhiteSpace(apiUrl))
				return default;

			if (!string.IsNullOrWhiteSpace(queryString))
			{
				apiUrl += apiUrl.Contains('?') ? $"&{queryString}" : $"?{queryString}";
			}

			if (liveFeed)
			{
				var accessToken = await _authenticationController.GetAccessTokenForLiveData();
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			}

			try
			{
				//Console.WriteLine("Calling URL: " + apiUrl);
				var response = await _httpClient.GetAsync(apiUrl);
				if (response.IsSuccessStatusCode)
				{
					var jsonData = await response.Content.ReadAsStringAsync();

					var result = JsonSerializer.Deserialize<T>(jsonData, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true,
						Converters = { new FlexibleDecimalListConverter() }
					});

					if(delayMs != null)
					{
						await Task.Delay(delayMs.Value);
					}
 					return result;
				}
				else
				{
					Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
					return default;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Request failed: {ex.Message}");
				return default;
			}
		}
	}
}
