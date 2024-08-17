using CommonFuncion.Dapper.Interfaces;
using CommonFuncion.Extensions;
using CommonFuncion.Results;

using Serilog;

namespace CommonFuncion.Logger
{
	public class LoggerHelper : ILoggerHelper
	{
		public LoggerHelper()
		{

		}

		/// <summary>
		/// Mensaje informativo en consola
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		public void Info(string Mensaje)
		{
			Log.Information(Mensaje);
		}
		/// <summary>
		/// Mensaje alerta en consola
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		public void Alert(string Mensaje)
		{
			Log.Warning(Mensaje);
		}

		/// <summary>
		/// Mensaje Error!! en consola
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		public void Error(string Mensaje)
		{
			Log.Error(Mensaje);
		}

		/// <summary>
		/// Inserta log en consola la validación del resultado encapsulado en DataResult de la BD con interfaz DbResult
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		public void ValidateResult<T>(DataResult<T> Model) where T : class, DbResult, new()
		{

			if (Model.Ok)
			{
				if (Model.Data.bResult)
				{
					Log.Information($"[{Model.Data.GetTypeValue}] - Resultado correcto");
				}
				else
				{
					Log.Warning($"[{Model.Data.GetType().Name}] - Alerta -[{Model.Data.vchMessage}]");
				}
			}
			else
			{
				Log.Error($"[ ERROR - {Model.Msg} ]  ");
			}
		}

		/// <summary>
		/// Inserta log en consola la validación de la lista resultado encapsulado en DataResult de la BD con interfaz DbResult
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		public void ValidateResult<T>(DataResult<IEnumerable<T>> Model) where T : class, DbResult, new()
		{
			if (Model.Ok)
			{
				var validaLsita = Model.Data.ValidaResult();

				if (validaLsita.Item1)
				{
					Log.Information($"[{Model.Data.GetType().Name}] - Resultado correcto");
				}
				else
				{
					Log.Warning($"[{Model.Data.GetType().Name}] - Alerta -[{validaLsita.Item2}]");
				}
			}
			else
			{
				Log.Error($"[ ERROR - {Model.Msg}]  ");
			}
		}

		/// <summary>
		/// Inserta log en consola la validación del modelo resultado de la BD con interfaz DbResult
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		public void ValidateSPResult<T>(T? Model) where T : class, DbResult, new()
		{
			if (Model.IsNull())
			{
				if (Model.bResult)
				{
					Log.Information($"[{Model.ToString()}] - Resultado correcto");
				}
				else
				{
					Log.Warning($"[{Model.GetType().Name}] - Alerta -[{Model.vchMessage}]");
				}
			}
			else
			{
				Log.Error($"[{Model.GetType().Name}] - Error -- Modelo es Nulo]");

			}
		}

		/// <summary>
		/// Valida si la lista que se espera por resultado contiene datos 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		/// <param name="Name"></param>
		public void ValidaLista<T>(IEnumerable<T> Model, string Name)
		{
			if (Model.IsNull())
			{
				Log.Warning($"[{Name}] - Alerta - Lista Nula]");
			}
			else if (Model.ToList().Count == 0)
			{
				Log.Warning($"[{Name}] - Alerta - Lista vacía]");
			}
			else
			{
				Log.Information($"[{Name}] - Info - [{Model.Count()}]]");
			}
		}

		/// <summary>
		/// Valida si el objeto que se espera por resultado contiene datos 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		/// <param name="Name"></param>
		public void ValidaObj<T>(T Model, string Name)
		{
			if (Model.IsNull())
			{
				Log.Warning($"[{Name}] - Alerta - Objeto Nulo]");
			}
			else
			{
				Log.Information($"[{Name}] - Info - [{Model.ToString()}]]");
			}
		}
	}
}