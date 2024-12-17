using Dapper;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using System.Data;

namespace EngramaCore.Dapper
{

	public class DapperManager : IDapperManager
	{
		private readonly string _defaultConnectionString;
		private readonly ILogger<DapperManager> _logger;

		public DapperManager(ILogger<DapperManager> logger)
		{
			_logger = logger;
		}



		public async Task<T?> GetAsync<T>(string query, string connectionString)
		{
			try
			{
				await using var db = new SqlConnection(connectionString);
				await db.OpenAsync();

				return await db.ExecuteScalarAsync<T>(new CommandDefinition(query, null, commandTimeout: 250, commandType: CommandType.Text));
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "An error occurred while executing the query: {Query}", query);
				throw;
			}
		}

		public async Task<IEnumerable<T>> GetAllAsync<T>(string query, string connectionString)
		{
			try
			{
				await using var db = new SqlConnection(connectionString);
				await db.OpenAsync();

				return await db.QueryAsync<T>(new CommandDefinition(query, null, commandTimeout: 250, commandType: CommandType.Text));
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "An error occurred while fetching all items for query: {Query}", query);
				throw;
			}
		}

		public async Task<T?> GetAsync<T>(string storedProcedure, DynamicParameters parameters, string connectionString)
		{
			try
			{
				await using var db = new SqlConnection(connectionString);
				await db.OpenAsync();

				return (await db.QueryAsync<T>(storedProcedure, parameters, commandTimeout: 250, commandType: CommandType.StoredProcedure)).FirstOrDefault();
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "An error occurred while executing the stored procedure: {StoredProcedure}", storedProcedure);
				throw;
			}
		}

		public async Task<IEnumerable<T>> GetAllAsync<T>(string storedProcedure, DynamicParameters parameters, string connectionString)
		{
			try
			{
				await using var db = new SqlConnection(connectionString);
				await db.OpenAsync();

				return await db.QueryAsync<T>(storedProcedure, parameters, commandTimeout: 250, commandType: CommandType.StoredProcedure);
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "An error occurred while fetching all items for stored procedure: {StoredProcedure}", storedProcedure);
				throw;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Release managed resources if any
			}
		}
	}
}