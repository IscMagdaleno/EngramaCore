using System.Globalization;

namespace EngramaCore
{
	public class Defaults
	{
		public static CultureInfo MX_CultureInfo { get; } = new("es-MX");

		public static string TipoVariables { get; } = "ValorNoEcontrado";

		public static DateTime SqlMinDate()
		{
			return new DateTime(1900, 1, 1);
		}
	}
}
