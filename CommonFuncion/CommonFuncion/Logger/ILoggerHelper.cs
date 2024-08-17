using CommonFuncion.Dapper.Interfaces;
using CommonFuncion.Results;

namespace CommonFuncion.Logger
{
	public interface ILoggerHelper
	{
		/// <summary>
		/// Mensaje informativo en consola
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		void Info(string Mensaje);

		/// <summary>
		/// Mensaje alerta en consola
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		void Alert(string Mensaje);

		/// <summary>
		/// Mensaje Error!! en consola
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		void Error(string Mensaje);

		/// <summary>
		/// Inserta log en consola la validación del resultado encapsulado en DataResult de la BD con interfaz DbResult
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		public void ValidateResult<T>(DataResult<T> Model) where T : class, DbResult, new();

		/// <summary>
		/// Inserta log en consola la validación de la lista resultado encapsulado en DataResult de la BD con interfaz DbResult
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		void ValidateResult<T>(DataResult<IEnumerable<T>> Model) where T : class, DbResult, new();

		/// <summary>
		/// Inserta log en consola la validación del modelo resultado de la BD con interfaz DbResult
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		void ValidateSPResult<T>(T? Model) where T : class, DbResult, new();

		void ValidaLista<T>(IEnumerable<T> Model, string Name);

		void ValidaObj<T>(T Model, string Name);
	}
}