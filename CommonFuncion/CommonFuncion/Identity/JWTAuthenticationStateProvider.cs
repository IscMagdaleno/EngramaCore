using CommonFuncion.Extensions;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace CommonFuncion.Identity
{

	public class JWTAuthenticationStateProvider : AuthenticationStateProvider, ILoginService
	{
		private readonly IJSRuntime js;
		private readonly HttpClient httpClient;
		private readonly string TokenKey = "TK";

		private AuthenticationState Anonymous
			=> new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

		public JWTAuthenticationStateProvider(IJSRuntime js, HttpClient httpClient)
		{
			this.js = js;
			this.httpClient = httpClient;
		}
		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{

			var token = await js.GetFromLocalStorage(TokenKey);
			if (token.IsEmpty())
			{
				return Anonymous;
			}


			return BuildAuthenticacionState(token);

		}


		public AuthenticationState BuildAuthenticacionState(string token)
		{
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));

		}


		private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
		{
			var claims = new List<Claim>();
			var payload = jwt.Split('.')[1];
			var jsonBytes = ParseBase64WithoutPadding(payload);
			var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

			keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

			if (roles != null)
			{
				if (roles.ToString().Trim().StartsWith("["))
				{
					var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

					foreach (var parsedRole in parsedRoles)
					{
						claims.Add(new Claim(ClaimTypes.Role, parsedRole));
					}
				}
				else
				{
					claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
				}

				keyValuePairs.Remove(ClaimTypes.Role);
			}

			claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
			return claims;
		}

		private byte[] ParseBase64WithoutPadding(string base64)
		{
			switch (base64.Length % 4)
			{
				case 2: base64 += "=="; break;
				case 3: base64 += "="; break;
			}
			return Convert.FromBase64String(base64);
		}

		public async Task LogIn(string token)
		{
			await js.SetInLocalStorage(TokenKey, token);

			var authState = BuildAuthenticacionState(token);
			NotifyAuthenticationStateChanged(Task.FromResult(authState));
		}

		public async Task LogOut()
		{
			await js.RemoveLocalStorage(TokenKey);

			httpClient.DefaultRequestHeaders.Authorization = null;
			NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
		}
	}
}

