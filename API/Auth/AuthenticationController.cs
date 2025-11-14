using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OpenF1Dashboard.API.Auth
{
	public class AuthenticationController
	{
		private string _TOKEN_API;
		private string _username;
		private string _password;
		private bool _isAuthenticated;
		private int _expiresIn; //1h
		private DateTimeOffset _tokenRequestTime;

		public AuthenticationController()
		{
			_TOKEN_API = "https://api.openf1.org/token";
			_username = "username";
			_password = "password";
			_isAuthenticated = false;
			_expiresIn = 0; //1h
		}

		private bool IsAuthenticated()
		{
			return _isAuthenticated && (_tokenRequestTime - DateTimeOffset.UtcNow).Seconds < _expiresIn;
		}

		public async Task<string> GetAccessTokenForLiveData()
		{
			if( IsAuthenticated() )
			{
				return _TOKEN_API;
			}
			else
			{
				await RefreshOAuth2Token();
				return _TOKEN_API;
			}
		}

		private async Task RefreshOAuth2Token()
		{
			using var client = new HttpClient();

			var requestContent = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("username", _username),
				new KeyValuePair<string, string>("password", _password),
			});

			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			try
			{
				var response = await client.PostAsync(_TOKEN_API, requestContent);
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var json = System.Text.Json.JsonDocument.Parse(content);
					var token = json.RootElement.GetProperty("access_token").GetString();
					var expiresIn = json.RootElement.GetProperty("expires_in").GetInt32();

					_isAuthenticated = true;
					_tokenRequestTime = DateTimeOffset.UtcNow;
					_expiresIn = expiresIn;

					Console.WriteLine($"Access token: {token}");
					Console.WriteLine($"Expires in: {expiresIn} seconds");

				}
				else
				{
					Console.WriteLine($"Error obtaining token: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
					_isAuthenticated= false;
					_expiresIn = 0;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Request failed: {ex.Message}");
				_isAuthenticated= false;
				_expiresIn = 0;
			}
		}
	}
}