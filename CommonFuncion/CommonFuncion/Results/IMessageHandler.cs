namespace CommonFuncion.Results
{
	public interface IMessageHandler
	{
		public Task<Result> InfoAsync(string subtitle, string message);

		public Task<Result> WarningAsync(string subtitle, string message);

		public Task<Result> ErrorAsync(string subtitle, Error error);
	}
}
