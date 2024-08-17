namespace CommonFuncion.Results
{
	public class DataResult<TResult>
	{
		public bool Ok { get; }

		public string Msg { get; } = string.Empty;

		public Exception Ex { get; } = new();

		public Error Error { get; } = Error.Empty();

		public TResult Data { get; }

		private DataResult(bool ok, TResult data)
		{
			Ok = ok;
			Data = data;
		}

		private DataResult(bool ok, string msg, Exception ex)
		{
			Ok = ok;
			Msg = msg;
			Ex = ex;
		}

		private DataResult(bool ok, Error error, Exception ex)
		{
			Ok = ok;
			Error = error;
			Msg = error.Message();
			Ex = ex;
		}

		public DataResult()
		{
		}

		public static DataResult<TResult> Success(TResult data)
		{
			return new DataResult<TResult>(true, data);
		}

		public static DataResult<TResult> Fail(string devMsg = "", Exception ex = null)
		{
			return new DataResult<TResult>(false, devMsg, ex);
		}

		public static DataResult<TResult> Fail(Error error, Exception ex = null)
		{
			return new DataResult<TResult>(false, error, ex);
		}
	}
}
