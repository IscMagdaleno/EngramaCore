using CommonFuncion.Dapper.Interfaces;
using CommonFuncion.Results;

namespace CommonFuncion.Dapper
{
	public interface IDapperManagerHelper
	{
		Task<DataResult<IList<TResult>>> GetAllAsync<TResult>(string Script, string conectionString = null) where TResult : class, DbResult, new();

		Task<DataResult<IList<TResult>>> GetAllAsync<TResult, TRequest>(TRequest request, string? conectionString = null)
			where TResult : class, DbResult, new()
			where TRequest : SpRequest;

		Task<DataResult<IList<TResult>>> GetAllCombinedAsync<TResult, TRequest>(string connectionString, TRequest request)
			where TResult : class, DbResult, new()
			where TRequest : SpRequest;

		Task<DataResult<IList<TResult>>> GetAllFromListAsync<TResult, TRequest>(string connectionString, string storeProcedure, string TypeName, IList<TRequest> model)
			where TResult : class, DbResult, new()
			where TRequest : SpRequest;

		Task<DataResult<TResult>> GetAsync<TResult>(string Script, string conectionString = null) where TResult : class, DbResult, new();

		Task<DataResult<TResult>> GetAsync<TResult, TRequest>(TRequest request, string? conectionString = null)
			where TResult : class, DbResult, new()
			where TRequest : SpRequest;

		Task<DataResult<TResult>> GetCombinedAsync<TResult, TRequest>(TRequest request, string? connectionString = null)
			where TResult : class, DbResult, new()
			where TRequest : SpRequest;


		Task<DataResult<TResult>> GetFromListAsync<TResult, TRequest>(string connectionString, string storeProcedure, string TypeName, IList<TRequest> model)
			where TResult : class, DbResult, new()
			where TRequest : SpRequest;
	}
}
