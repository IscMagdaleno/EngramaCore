namespace CommonFuncion
{
	public sealed class Error
	{
		private ErrorType _type;

		private string _code = string.Empty;

		private string _msg = string.Empty;

		private string _executableString = string.Empty;

		private static string CommonDatabaseErrorMsg => "Ocurrió un problema de comunicación con la base de datos, contacte a su administrador para darle solución";

		private static string TimeoutErrorMsg => "El tiempo de espera se ha excedido en la base de datos, contacte al administrador para darle solución";

		private Error() { }

		public static Error Empty()
		{
			return new Error();
		}

		public static Error DBException(int code, string executableString = "")
		{
			return new Error
			{
				_type = ErrorType.Database,
				_code = $"DB-{code}",
				_executableString = executableString,
				_msg = CommonDatabaseErrorMsg + ". Fecha: " + DateTime.Now
			};
		}

		public static Error Exception(string code, string msg, string execString = "")
		{
			return new Error
			{
				_type = ErrorType.Exception,
				_code = $"EX-{code}",
				_executableString = execString,
				_msg = msg + ". Fecha: " + DateTime.Now
			};
		}

		public static Error Function(string code, string msg)
		{
			return new Error
			{
				_type = ErrorType.Function,
				_code = $"FN-{code}",
				_msg = msg + " " + DateTime.Now
			};
		}

		public ErrorType Type()
		{
			return _type;
		}

		public string Code()
		{
			return _code;
		}

		public string Message()
		{
			return _msg;
		}

		public string ExecutableString()
		{
			return _executableString;
		}

		public enum ErrorType
		{
			Database,

			Exception,

			Function
		}
	}
}
