using Newtonsoft.Json;

namespace CommonFuncion
{
	public sealed class Jsons
	{
		public static string Stringify(object obj, bool bRemoveNull = false, Formatting formatting = Formatting.None)
		{
			var jsonSettings = new JsonSerializerSettings();
			if (bRemoveNull)
			{
				jsonSettings.NullValueHandling = NullValueHandling.Ignore;
			}

			return JsonConvert.SerializeObject(obj, formatting, jsonSettings);
		}

		public static T Parse<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}


		public static T ReadJsonPath<T>(string Path)
		{
			using (StreamReader r = new StreamReader(Path))
			{
				string json = r.ReadToEnd();
				return JsonConvert.DeserializeObject<T>(json);
			}
		}

		public static List<T> ReadListJsonPath<T>(string Path)
		{
			try
			{

				using (StreamReader r = new StreamReader(Path))
				{

					string json = r.ReadToEnd();
					var jsonResult = JsonConvert.DeserializeObject<List<T>>(json);
					return jsonResult;
				}
			}
			catch (Exception e)
			{
				return new List<T>();
			}
		}

	}
}
