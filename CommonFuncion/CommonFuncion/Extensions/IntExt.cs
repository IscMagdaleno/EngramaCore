namespace CommonFuncion.Extensions
{
	public static class IntExt
	{
		public static bool IsAny(this int value, params int[] values)
		{
			return values.Count(val => val == value) > 0;
		}

		public static bool Between(this int value, int a, int b)
		{
			return value >= a && value <= b;
		}

		public static bool NotBetween(this int value, int a, int b)
		{
			return value < a && value > b;
		}

		public static bool NotAny(this int value, params int[] values)
		{
			return values.All(val => value != val);
		}

		public static bool AreAll(this int value, params int[] values)
		{
			return values.All(val => value == val);
		}

		public static bool NotZeroOrLess(this int value)
		{
			return value > 0;
		}

		public static bool ZeroOrLess(this int i)
		{
			return i <= 0;
		}

		public static bool LessThanZero(this int i)
		{
			return i < 0;
		}

		public static string GetMonth(this int month)
		{
			var months = new List<string>
			{
				"Enero", "Febrero", "Marzo",
				"Abril", "Mayo", "Junio",
				"Julio", "Agosto", "Septiembre",
				"Octubre", "Noviembre", "Diciembre"
			};

			return months[month > 0 && month < 13 ? month - 1 : 0].ToUpper();
		}

		public static string GetDay(this int day)
		{
			var days = new List<string>
			{
				"Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo"
			};

			return days[(day > 0 && day < 8 ? day - 1 : 0)];
		}

		public static bool EqualsZero(this int i)
		{
			return i == 0;
		}

		public static int EqualsZeroSetDefault(this int self, int def = -1)
		{
			return self.EqualsZero() ? def : self;
		}

		public static int EqualsLessThanZero(this int self, int def = -1)
		{
			return self.LessThanZero() ? def : self;
		}

		public static bool InRange(this int value, int start, int end)
		{
			return Enumerable.Range(start, end).Contains(value);
		}

		public static bool NotInRange(this int value, int start, int end)
		{
			return Enumerable.Range(start, end).Contains(value).False();
		}

		public static int ReplaceIf(this int self, int value, int newValue)
		{
			return self.Equals(value) ? newValue : self;
		}

		public static int IndexOfRange(this int self, int[] values)
		{
			var start = 0;

			for (var i = 0; i < values.Length; i++)
			{
				var end = values[i];

				if (self >= start && self <= end)
				{
					return i;
				}

				start = values[i];
			}

			return -1;
		}
	}
}
