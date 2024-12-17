using EngramaCore.Dapper.Interfaces;
using EngramaCore.Results;

namespace EngramaCore.Dapper
{
	public interface IDapperManagerHelper
	{


		Task<DataResult<TResult>> GetAsync<TResult>(string Script, string connectionString = null) where TResult : class, DbResult, new();
		Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult>(string script, string? connectionString = null) where TResult : class, DbResult, new();


		Task<DataResult<TResult>> GetAsync<TResult, TRequest>(TRequest request, string? connectionString = null)
			where TResult : class, DbResult, new()
			where TRequest : SpRequest;
		Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult, TRequest>(TRequest request, string? connectionString = null)
			where TResult : class, DbResult, new()
			where TRequest : SpRequest;


		Task<DataResult<TResult>> GetWithListAsync<TResult, TRequest>(TRequest request, string? connectionString = null)
			where TRequest : SpRequest
			where TResult : class, DbResult, new();

		Task<DataResult<IEnumerable<TResult>>> GetAllWithListAsync<TResult, TRequest>(TRequest request, string? connectionString = null)
			where TResult : class, DbResult, new()
			where TRequest : SpRequest;
	}
}
