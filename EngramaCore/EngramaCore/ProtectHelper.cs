using Blazor.SubtleCrypto;

using Microsoft.JSInterop;

namespace EngramaCore
{
	public sealed class ProtectHelper
	{
		private readonly string KeySecret = "abc123zyx987";

		private CryptoService CryptoService;

		public ProtectHelper(IJSRuntime jSRuntime)
		{
			CryptoService = new CryptoService(jSRuntime,
				new CryptoOptions
				{
					Key = KeySecret
				}
			);
		}

		public async Task<string> EncryptAsync(string text)
		{
			CryptoResult result = await CryptoService.EncryptAsync(text);

			return result.Value.ToString();
		}

		public async Task<string> DecryptAsync(string text)
		{
			string result = await CryptoService.DecryptAsync(text);

			return result;
		}
	}
}