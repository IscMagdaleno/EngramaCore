using CommonFuncion.Extensions;

using Dapper;

using Microsoft.Extensions.Configuration;

using System.Data;
using System.Data.SqlClient;

namespace CommonFuncion.Dapper
{
	public class DapperManager : IDapperManager
	{
		public string ConnectionString { get; set; }


		public DapperManager(IConfiguration configuration)
		{
			ConnectionString = configuration.GetConnectionString("EngramaCloudConnection");
		}


		public T? Get<T>(string Script, string conectionString = null)
		{
			if (conectionString.NotEmpty())
			{
				ConnectionString = conectionString;
			}
			using (SqlConnection db = new SqlConnection(ConnectionString))
			{
				var resultado = db.ExecuteScalar<T>(Script, commandTimeout: 250, commandType: CommandType.Text);
				SqlConnection.ClearPool(db);

				return resultado;
			}
		}

		public IList<T> GetAll<T>(string Script, string? conectionString = null)
		{
			if (conectionString.NotEmpty())
			{
				ConnectionString = conectionString;
			}

			using (SqlConnection db = new SqlConnection(ConnectionString))
			{
				var resultado = db.Query<T>(Script, commandTimeout: 250, commandType: CommandType.Text).ToList();
				SqlConnection.ClearPool(db);

				return resultado;
			}
		}

		public T? Get<T>(string sp, DynamicParameters dynamicParameters, string? conectionString = null)
		{
			if (conectionString.NotEmpty())
			{
				ConnectionString = conectionString;
			}

			using (SqlConnection db = new SqlConnection(ConnectionString))
			{
				var result = db.Query<T>(sp, dynamicParameters, commandTimeout: 250, commandType: CommandType.StoredProcedure).FirstOrDefault();
				SqlConnection.ClearPool(db);
				return result;
			}
		}

		public IList<T> GetAll<T>(string sp, DynamicParameters dynamicParameters, string? conectionString = null)
		{
			if (conectionString.NotEmpty())
			{
				ConnectionString = conectionString;
			}

			using (SqlConnection db = new SqlConnection(ConnectionString))
			{
				var result = db.Query<T>(sp, dynamicParameters, commandTimeout: 250, commandType: CommandType.StoredProcedure).ToList();
				SqlConnection.ClearPool(db);
				return result;
			}
		}

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing) { }
		}
	}
}
