namespace CommonFuncion.Extensions
{
	public static class DictionaryExt
	{
		public static bool SingleOne<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
			{
				return false;
			}

			return dictionary.Count == 1;
		}

		public static bool IsEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
			{
				return true;
			}

			return dictionary.Count == 0;
		}

		public static bool NotEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
			{
				return true;
			}

			return dictionary.Count != 0;
		}

		public static Dictionary<TKey, TValue> Copy<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
		{
			return new Dictionary<TKey, TValue>(dictionary);
		}

		public static IDictionary<TKey, TValue> Update<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			dictionary[key] = value;

			return dictionary;
		}

		public static bool NoContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			return dictionary.ContainsKey(key).False();
		}

		public static TValue GetValueBy<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
		{
			var value = new TValue();

			dictionary.TryGetValue(key, out value);

			return value;
		}
	}
}
