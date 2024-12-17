namespace EngramaCore.Extensions
{
	public static class DecimalExt
	{
		public static bool IsAny(this decimal value, params decimal[] values)
		{
			return values.Count(val => val == value) > 0;
		}

		public static string ToDecimalCurrency(this decimal value, bool bValidateZeroByEmpty = false)
		{
			if (bValidateZeroByEmpty)
			{
				return value.Equals(0) ? string.Empty : $"{value:C2}";
			}

			return $"{value:C2}";
		}

		public static decimal Trim(this decimal value, int decimales)
		{
			var split = value.ToString().Split('.');
			var indexOfDot = split[1].Length;

			if (indexOfDot > decimales)
			{
				split[1] = split[1].Substring(0, decimales);
				value = Convert.ToDecimal($"{split[0]}.{split[1]}");
			}

			return value;
		}
	}
}
