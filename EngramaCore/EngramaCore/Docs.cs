namespace EngramaCore
{
	public sealed class Docs
	{
		public static string ReadDocInString(string Path)
		{
			var Respuesta = string.Empty;
			if (File.Exists(Path))
			{
				using (StreamReader r = new StreamReader(Path))
				{
					Respuesta += r.ReadToEnd();
				}
				/*
				Respuesta = Respuesta.Replace("\t", " ");
				Respuesta = Respuesta.Replace("\r\n", " ");
				Respuesta = Respuesta.Replace("\r", " ");
				*/
			}

			return Respuesta;
		}

		public static List<string> ReadDocInList(string Path)
		{
			var Respuesta = new List<string>();
			if (File.Exists(Path))
			{
				using (StreamReader r = new StreamReader(Path))
				{
					Respuesta.Add(r.ReadToEnd());
				}
			}

			return Respuesta;
		}
	}
}

