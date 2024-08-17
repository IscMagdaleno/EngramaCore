using QRCoder;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CommonFuncion.Extensions
{
	public static class StringExt
	{
		public static bool IsEmpty(this string value)
		{
			return string.IsNullOrEmpty(value);
		}

		public static bool NotEmpty(this string value)
		{
			return string.IsNullOrEmpty(value).False();
		}

		public static bool Length(this string value, int length)
		{
			return value.Length == length;
		}

		public static string MaxSubstring(this string value, int length)
		{
			if (value.Length > length)
			{
				return value[..length];
			}

			return value;
		}

		public static bool ToBool(this string value, in string p, in string n)
		{
			if (string.Equals(value, p, StringComparison.CurrentCultureIgnoreCase))
			{
				return true;
			}

			if (string.Equals(value, n, StringComparison.CurrentCultureIgnoreCase))
			{
				return false;
			}

			return false;
		}

		public static bool IsAny(this string value, params string[] values)
		{
			return values.Count(val => val == value) > 0;
		}

		public static bool ContainsAny(this string value, params string[] values)
		{
			return values.Count(val => val.Contains(value)) > 0;
		}

		public static bool NotAny(this string value, params string[] values)
		{
			return values.All(val => value != val);
		}

		public static string TrimAll(this string value)
		{
			if (value.IsEmpty())
			{
				value = string.Empty;
			}

			return value.Replace(" ", "");
		}

		public static bool Match(this string value, string pattern)
		{
			return Regex.IsMatch(value, pattern);
		}

		public static bool NotMatch(this string value, string pattern)
		{
			return Regex.IsMatch(value, pattern).False();
		}

		public static bool HasDigits(this string value)
		{
			if (value.IsEmpty())
			{
				return false;
			}

			return value.Any(char.IsDigit);
		}

		public static bool HasLetters(this string value)
		{
			if (value.IsEmpty())
			{
				return false;
			}

			return value.Any(char.IsLetter);
		}

		public static bool ToDateTime(this string value)
		{
			var datetime = Defaults.SqlMinDate();

			return DateTime.TryParseExact(
				value, "dd/MM/YYYY", Defaults.MX_CultureInfo, DateTimeStyles.AssumeUniversal,
				out datetime
			);
		}

		public static DateTime ToDateMX(this string value)
		{
			if (value.IsEmpty())
			{
				return Defaults.SqlMinDate();
			}

			return Convert.ToDateTime(value, Defaults.MX_CultureInfo);
		}

		public static string NewGuid(this string value, int n = 20)
		{
			return Guid.NewGuid().ToString()[..n];
		}

		public static string SingleValue(this string value, string separator, int index)
		{
			if (value.NotEmpty() && value.Contains(separator) && index < value.Split().Length)
			{
				return value.Split(separator).ElementAt(index);
			}

			return value;
		}

		public static string UpperSeparator(this string self, string separator)
		{
			var value = string.Empty;

			foreach (var item in self.ToCharArray())
			{
				if (char.IsUpper(item))
				{
					value += separator;
				}

				value += item;
			}

			return value;
		}

		public static string WrapText(this string self, int n)
		{
			var value = self;

			if (self.NotNull() && self.Length > n)
			{
				value = self[..n] + "...";
			}

			return value;
		}

		public static IList<string> UpperSplit(this string self, bool skipFirst = true)
		{
			if (skipFirst)
			{
				return self.UpperSeparator(" ").Split(" ").Skip(1).ToList();
			}

			return self.UpperSeparator(" ").Split(" ");
		}

		public static string ClearFormatFilename(this string self)
		{
			var invalids = Path.GetInvalidFileNameChars();
			var newName = string.Join("_", self.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');

			return newName;
		}

		public static string UrlDecode(this string self)
		{
			return HttpUtility.UrlDecode(self, Encoding.UTF8);
		}

		public static string UrlEncode(this string self)
		{
			return HttpUtility.UrlEncode(self, Encoding.UTF8);
		}

		public static string ToUrl(this string self)
		{
			return new Uri(self).ToString();
		}

		public static string RemoveConcatenation(this string self)
		{
			var index = self.IndexOf("-") + 1;

			if (index > 0)
			{
				return self.Substring(index, (self.Length - index)).TrimStart();
			}

			return self;
		}

		public static string ReplaceFirst(this string text, string search, string replace)
		{
			int position = text.IndexOf(search);

			if (position < 0)
			{
				return text;
			}

			return text[..position] + replace + text[(position + search.Length)..];
		}

		public static string RandomsWords(this string self, int start = 5, int end = 15, int length = 100)
		{
			var words = new List<string>();

			for (int i = 0; i < length; i++)
			{
				var iRand = new Random().Next(5, 15);
				var word = "";

				for (int j = 0; j < iRand; j++)
				{
					var upper = new Random().Next(65, 90);
					var lowwer = new Random().Next(97, 122);

					word += (char)(upper % j == 0 ? upper : lowwer);
				}

				words.Add(word);
			}

			return string.Join(" ", words);
		}

		public static string TSQLIndexToCSharp(this string index)
		{
			switch (index)
			{
				case "b":

					return "bool";

				case "bi":

					return "long";

				case "d":
				case "dt":

					return "DateTime";

				case "i":

					return "int";

				case "fl":
				case "flt":

					return "float";

				case "m":

					return "decimal";

				case "sm":

					return "short";

				case "ti":

					return "byte";

				case "vch":

					return "string";

				case "t":

					return "Time";

				default:

					return "int";
			}
		}

		public static string RemoveFromThisTo(this string self, string index, int start = 0)
		{
			var end = self.IndexOf(index);
			var value = self.Substring(start, end);

			return value;
		}

		public static string Capitalize(this string word)
		{
			return $"{word[..1].ToUpper()}{word[1..].ToLower()}";
		}

		public static bool IsWord(this string self)
		{
			return self.Count(x => x.IsLetter()) > 0;
		}

		public static bool AreNumbers(this string self)
		{
			return self.Count(x => x.IsNumber()) == self.Length;
		}

		public static string ReplaceIf(this string self, string value, string newValue)
		{
			return self.Equals(value) ? newValue : self;
		}

		public static string Repeat(this string self, char character, int length)
		{
			if (length > 0)
			{
				return new string(character, length);
			}

			return self;
		}

		public static string CompleteWithSpaces(this string self, int length, bool left)
		{
			var value = self;
			var spaces = length - value.Length;
			var repeated = value.Repeat(' ', spaces);

			if (self.Length < length)
			{
				value = left ? repeated + self : self + repeated;
			}
			else if (self.Length > length)
			{
				value = value[..length];
			}

			return value;
		}

		public static string RegexReplace(this string self, string pattern, string replacement)
		{
			return Regex.Replace(self, pattern, replacement);
		}

		public static string ToTimeHalfDay(this string self)
		{
			var datetime = DateTime.Parse(self);

			return datetime.ToString("hh:mm:ss");
		}

		public static string LastSubstring(this string self, string search)
		{
			var lastIndexOf = self.LastIndexOf(search);

			var lastSubstring = self[(lastIndexOf + 1)..];

			return lastSubstring;
		}

		public static bool IsMatch(this string self, string pattern)
		{
			return Regex.IsMatch(self, pattern);
		}

		public static byte[] ToQrCodeBytesArray(this string self, int pixelsPerModule = 2)
		{
			using (var qrGenerator = new QRCodeGenerator())
			{
				using (var qrCodeData = qrGenerator.CreateQrCode(self, QRCodeGenerator.ECCLevel.L))
				{
					using (var qRCode = new PngByteQRCode(qrCodeData))
					{
						return qRCode.GetGraphic(pixelsPerModule);
					}
				}
			}
		}
	}
}
