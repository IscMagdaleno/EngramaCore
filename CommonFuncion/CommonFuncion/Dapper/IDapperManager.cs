using Dapper;

namespace CommonFuncion.Dapper
{
	public interface IDapperManager
	{
		T? Get<T>(string String, string conectionString = null);

		T? Get<T>(string sp, DynamicParameters dynamicParameters, string conectionString = null);

		IList<T> GetAll<T>(string sp, DynamicParameters dynamicParameters, string conectionString = null);

		IList<T> GetAll<T>(string String, string conectionString = null);
	}
}
