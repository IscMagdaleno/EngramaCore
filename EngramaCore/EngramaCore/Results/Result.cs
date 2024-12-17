namespace EngramaCore.Results
{
	public sealed class Result
	{
		public bool Ok { get; }

		public string Msg { get; } = string.Empty;

		public Exception Ex { get; } = new();

		public Error Error { get; } = Error.Empty();

		private Result(bool ok)
		{
			Ok = ok;
		}

		private Result(bool ok, string devMsg, Exception ex)
		{
			Ok = ok;
			Msg = devMsg;
			Ex = ex;
		}

		private Result(bool ok, Error error, Exception ex)
		{
			Ok = ok;
			Error = error;
			Msg = error.Message();
			Ex = ex;
		}

		public static Result Success()
		{
			return new Result(true);
		}

		public static Result Fail(string devMsg = "", Exception ex = null)
		{
			return new Result(false, devMsg, ex);
		}

		public static Result Fail(Error error, Exception ex = null)
		{
			return new Result(false, error, ex);
		}
	}
}
