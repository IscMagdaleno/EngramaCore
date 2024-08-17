namespace CommonFuncion.Extensions
{
	public static class DatetimeExt
	{
		public static bool IsAny(this DateTime value, params DateTime[] values)
		{
			return values.Count(val => val == value) > 0;
		}

		public static string ToFormatMX(this DateTime value, bool bWithSlash = true)
		{
			if (bWithSlash)
			{
				return Convert.ToDateTime(value.ToString()).ToString("dd/MM/yyyy");
			}
			else
			{
				return Convert.ToDateTime(value.ToString()).ToString("dd-MM-yyyy");
			}
		}

		public static string ToFormatMXComplete(this DateTime value, bool bWithSlash = true)
		{
			if (bWithSlash)
			{
				return Convert.ToDateTime(value.ToString()).ToString("dd/MM/yyyy hh:mm:ss tt");
			}

			return Convert.ToDateTime(value.ToString()).ToString("dd-MM-yyyy hh:mm:ss tt");
		}

		public static bool EqualsToDefaultDate(this DateTime datetime)
		{
			var dDate = DateTime.Parse(datetime.ToShortDateString());
			var dDefault = Defaults.SqlMinDate();

			var value = DateTime.Compare(dDefault, dDate);

			return value == 0;
		}

		public static bool EqualsToDefaultDateTime(this DateTime datetime, bool defaultFirstValue = false)
		{
			var value = -1;

			if (defaultFirstValue)
			{
				value = DateTime.Compare(datetime, Defaults.SqlMinDate());
			}
			else
			{
				value = DateTime.Compare(Defaults.SqlMinDate(), datetime);
			}

			return value == 0;
		}

		public static string EqualsDefaultToFormatMXOrEmpty(this DateTime datetime)
		{
			var value = DateTime.Compare(Defaults.SqlMinDate(), datetime);

			return value == 0 ? string.Empty : datetime.ToFormatMX();
		}

		public static string ToNumber(this DateTime dateTime, bool time = true)
		{
			var sTime = time ? "hmmss" : "";

			return dateTime.ToString("ddMMyyyy" + sTime);
		}

		public static string ToFormatNull(this DateTime self, bool condition = false, string value = "N/A")
		{
			if (condition)
			{
				return value;
			}

			return self.ToFormatMX();
		}
	}
}
