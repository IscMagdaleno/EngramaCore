using EngramaCore.Results;

using System.Reflection;

namespace EngramaCore.Extensions
{
	public static class ObjectExt
	{
		public static int ToInt(this object obj)
		{
			int.TryParse(obj?.ToString(), out var value);

			return value;
		}

		public static long ToLong(this object obj)
		{
			long.TryParse(obj?.ToString(), out var value);

			return value;
		}

		public static ulong ToULong(this object obj)
		{
			ulong.TryParse(obj?.ToString(), out var value);

			return value;
		}

		public static DateTime ToDateTime(this object obj)
		{
			DateTime.TryParse(obj?.ToString(), out var value);

			return value;
		}

		public static decimal ToDecimal(this object obj)
		{
			decimal.TryParse(obj?.ToString(), out var value);

			return value;
		}

		public static double ToDouble(this object obj)
		{
			double.TryParse(obj?.ToString(), out var value);

			return value;
		}

		public static float ToFloat(this object obj)
		{
			float.TryParse(obj?.ToString(), out var value);

			return value;
		}

		public static short ToShort(this object obj)
		{
			short.TryParse(obj?.ToString(), out var value);

			return value;
		}

		public static bool ToBoolean(this object obj)
		{
			bool.TryParse(obj?.ToString(), out var value);

			return value;
		}

		public static bool IsNull(this object obj)
		{
			return obj == null;
		}

		public static bool NotNull(this object obj)
		{
			return obj != null;
		}

		public static DataResult<T> SearchProperty<T>(this object obj, string propertyName)
		{
			try
			{
				var properties = obj.GetType().GetProperties();
				var property = properties.FirstOrDefault(p => p.Name == propertyName);

				if (property.NotNull())
				{
					return DataResult<T>.Success((T)property.GetValue(obj));
				}
			}
			catch (Exception ex)
			{
				//Logger.Get.Exception(ex);
			}

			return DataResult<T>.Fail();
		}

		public static bool NotEquals(this object obj, object b)
		{
			return obj?.ToString() != b?.ToString();
		}

		public static object GetTypeValue(this object obj, string type)
		{
			switch (type)
			{
				case "DateTime":

					return Convert.ToDateTime(obj.ToString()).ToString("dd/MM/yyyy");

				case "String":

					return obj.ToString().NotEmpty() ? obj.ToString().Trim() : obj.GetDefaultProperty();

				default:

					return obj;
			}
		}

		public static object GetDefaultProperty(this object self)
		{
			var index = self.GetType().GetTypeInfo().FullName;

			switch (index)
			{
				case "System.Int":

					return "0";

				case "System.DateTime":

					return Defaults.SqlMinDate();

				case "System.Single":

					return 0.0;

				case "System.String":

					return "null";

				default:

					return "-1";
			}
		}

		public static object GetDataType(this object obj)
		{
			var value = obj.ToString();

			if (value.IsWord() || value.IsEmpty())
			{
				return obj.ToString();
			}
			else if (value.Length >= 20 && value.AreNumbers())
			{
				return obj.ToString();
			}
			else
			{
				if (value.Contains(".") && value.Contains("$").False())
				{
					return obj.ToFloat();
				}
				else if (value.Contains("$"))
				{
					var whithOutSign = value.Replace("$", "");

					return whithOutSign.ToDecimal();
				}
				else if (value.Contains("/"))
				{
					return obj.ToDateTime();
				}
				else if (value.Contains("\t"))
				{
					return Defaults.SqlMinDate();
				}
				else if (value.ToLong() > int.MaxValue)
				{
					return obj.ToLong();
				}
				else
				{
					return obj.ToInt();
				}
			}
		}

		public static bool NotAny(this object self, IEnumerable<object> values)
		{
			return values.Any(x => x.ToString().Equals(self.ToString())).False();
		}
	}
}
