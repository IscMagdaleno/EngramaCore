using CommonFuncion.Extensions;

namespace CommonFuncion.UnitTest
{
	public sealed class JsonUnitTest<T>
	{

		public static void GeneraJson(string PathSaveJson, IList<T> Data)
		{
			var datos = Jsons.Stringify(Data);

			if (File.Exists(PathSaveJson).False())
			{
				// Create a file to write to.
				using (StreamWriter sw = File.CreateText(PathSaveJson))
				{
					sw.Write(datos);
				}

				//using (StreamReader sr = File.OpenText(PathSaveJson))
				//{
				//	string s;
				//	while ((s = sr.ReadLine()) != null)
				//	{
				//		Console.WriteLine(s);
				//		Log.Information(s);
				//	}
				//}
			}

		}
	}



}
