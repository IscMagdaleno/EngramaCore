using Newtonsoft.Json.Linq;

namespace EngramaCore.Extensions
{
	public static class EnumerableExt
	{
		public static bool NotAny(this object self, IEnumerable<object> values)
		{
			return values.Any(x => x.ToString().Equals(self.ToString())).False();
		}

		public static bool IsEmpty<T>(this IEnumerable<T> list)
		{
			if (list == null)
			{
				return true;
			}

			return list.Any().False();
		}

		public static bool SingleOne<T>(this IEnumerable<T> list)
		{
			if (list == null)
			{
				return false;
			}

			return list.Count() == 1;
		}

		public static bool NotEmpty<T>(this IEnumerable<T> list)
		{
			if (list == null)
			{
				return false;
			}

			return list.Any();
		}

		public static bool AddIf<T>(this IList<T> lst, T data, Func<T, bool> check)
		{
			if (lst.All(check).False()) return false;

			lst.Add(data);

			return true;
		}

		public static bool AreEquals<T, K>(this IEnumerable<T> self, IEnumerable<K> another)
		{
			var a = Jsons.Stringify(self);
			var b = Jsons.Stringify(another);

			return a.Equals(b);
		}

		public static bool NotAny<T>(this IEnumerable<T> items, Func<T, bool> predicate)
		{
			return items.Any(predicate).False();
		}

		/*
		 * Extensiones para IList<T>
		 */

		public static IList<T> TrimSpaces<T>(this IList<T> self)
		{
			return self.Select(x => x.TrimSpaces()).ToList();
		}

		public static string ToStringJoin<T>(this IEnumerable<T> self)
		{
			return string.Join(", ", self);
		}

		public static string Join<T>(this IEnumerable<T> self, string separator = ", ")
		{
			return string.Join(separator, self);
		}

		public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this object self)
		{
			return JObject.FromObject(self).ToObject<Dictionary<TKey, TValue>>();
		}
	}
}
