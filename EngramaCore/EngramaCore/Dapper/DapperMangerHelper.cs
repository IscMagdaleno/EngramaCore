using Dapper;

using EngramaCore.Dapper.Interfaces;
using EngramaCore.Extensions;
using EngramaCore.Logger;
using EngramaCore.Results;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Data;
using System.Text;

namespace EngramaCore.Dapper
{
	/// <summary>
	/// Auxiliar para el uso de dapper al convertir los modelos ya en la lista de parámetros 
	/// </summary>
	public class DapperMangerHelper : IDapperManagerHelper
	{
		public IDapperManager DapperManager { get; set; }
		public ILoggerHelper LoggerHelper { get; }
		private readonly ILogger<DapperManager> _logger;

		/**/

		private readonly Dictionary<string, DbType> DbTypeMap = new();

		/**/

		private string ConnectionString { get; set; }


		public DapperMangerHelper(IDapperManager dapperManager, ILoggerHelper loggerHelper, IConfiguration configuration, ILogger<DapperManager> logger)
		{
			ConnectionString = configuration.GetConnectionString("EngramaCloudConnection");

			DapperManager = dapperManager;
			LoggerHelper = loggerHelper;
			_logger = logger;

			DbTypeMap.Add("System.Boolean", DbType.Boolean);

			DbTypeMap.Add("System.Byte", DbType.Byte);
			DbTypeMap.Add("System.SByte", DbType.SByte);
			DbTypeMap.Add("System.Byte[]", DbType.Binary);

			//DbTypeMap.Add("System.Char", DbType.);

			DbTypeMap.Add("System.Decimal", DbType.Decimal);
			DbTypeMap.Add("System.Double", DbType.Decimal);
			DbTypeMap.Add("System.Single", DbType.Decimal);

			DbTypeMap.Add("System.Int32", DbType.Int32);
			DbTypeMap.Add("System.UInt32", DbType.UInt32);

			//DbTypeMap.Add("System.IntPtr", DbType.Int32);
			//DbTypeMap.Add("System.UIntPtr", DbType.Int32);


			DbTypeMap.Add("System.Int64", DbType.Int64);
			DbTypeMap.Add("System.UInt64", DbType.UInt64);

			DbTypeMap.Add("System.Int16", DbType.Int16);
			DbTypeMap.Add("System.UInt16", DbType.UInt16);

			DbTypeMap.Add("System.Object", DbType.Object);
			DbTypeMap.Add("System.String", DbType.String);

			DbTypeMap.Add("System.Datetime", DbType.DateTime);
			DbTypeMap.Add("System.DateTime", DbType.DateTime);
		}

		#region Gets from script



		public async Task<DataResult<TResult>> GetAsync<TResult>(string Script, string? connectionString = null)
			where TResult : class, DbResult, new()
		{
			if (connectionString.IsEmpty())
			{
				connectionString = ConnectionString;
			}


			var resp = new DataResult<TResult>();
			if (string.IsNullOrWhiteSpace(Script))
			{
				var msg = ("The script cannot be null or empty.");
				resp = DataResult<TResult>.Fail(msg);
				return resp;
			}



			try
			{
				var data = await DapperManager.GetAsync<TResult>(Script, connectionString);

				data ??= new TResult { bResult = false, vchMessage = "Sin resultados" };


				LoggerHelper.ValidateSPResult(data);

				resp = (DataResult<TResult>.Success(data));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to execute script: {Script} with connection: {ConnectionString}", Script, connectionString);

				resp = DataResult<TResult>.Fail(ex.Message);

			}

			LoggerHelper.ValidateResult(resp);
			return resp;
		}

		public async Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult>(string script, string? connectionString = null)
	where TResult : class, DbResult, new()
		{
			var result = new DataResult<IEnumerable<TResult>>();

			if (connectionString.IsEmpty())
			{
				connectionString = ConnectionString;
			}

			var resp = new DataResult<TResult>();
			if (script.IsEmpty())
			{
				var msg = ("The script cannot be null or empty.");
				result = DataResult<IEnumerable<TResult>>.Fail(msg);
				return result;
			}


			try
			{
				// Execute the command asynchronously to get the data
				var dataList = await DapperManager.GetAllAsync<TResult>(script, connectionString);

				// Use the system collections string; if empty, add a 'no results' message
				if (dataList.IsEmpty())
				{
					dataList = new List<TResult> { new TResult { bResult = false, vchMessage = "Sin resultados" } };
				}

				// Indicate a successful operation
				result = DataResult<IEnumerable<TResult>>.Success(dataList);

				// Validate the received list of results
				LoggerHelper.ValidateResultLst(result);

			}
			catch (Exception ex)
			{
				// Log error without displaying sensitive script details
				_logger.LogError(ex, "Error executing script with {ConnectionDescription}", connectionString != null ? "custom connection string" : "default connection string");

				// Prepare the failed result with the error message
				result = DataResult<IEnumerable<TResult>>.Fail(ex.Message);
			}

			// Validate the entire result and log appropriately
			LoggerHelper.ValidateResultLst(result);

			return result;
		}

		#endregion Gets from script


		#region Gets from TRequest

		public async Task<DataResult<TResult>> GetAsync<TResult, TRequest>(TRequest request, string? connectionString = null)
	where TRequest : SpRequest
	where TResult : class, DbResult, new()
		{

			if (connectionString.IsEmpty())
			{
				connectionString = ConnectionString;
			}

			// Initialize result holder and stored procedure name
			var result = new DataResult<TResult>();
			var storedProcedure = $"[dbo].[{request.StoredProcedure}]";

			string executableString = string.Empty;

			try
			{
				// Create dynamic parameters from the request
				var dynamicParameters = CreateDynamicParameters(request);
				executableString = dynamicParameters.ExcecutableString;

				// Asynchronously retrieve data
				var data = await DapperManager.GetAsync<TResult>(storedProcedure, dynamicParameters.Parameters, connectionString);

				data ??= new TResult { bResult = false, vchMessage = "Sin resultados" };

				result = DataResult<TResult>.Success(data);
			}
			catch (SqlException sqlEx)
			{
				_logger.LogError(sqlEx, "SQL error executing procedure: {StoredProcedure} with {ExecutableString}", storedProcedure, executableString);
				result = DataResult<TResult>.Fail(executableString, sqlEx);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error executing procedure: {StoredProcedure} with {ExecutableString}", storedProcedure, executableString);
				result = DataResult<TResult>.Fail(ex.Message);
			}

			// Log and validate the operation's result
			LoggerHelper.ValidateResult(result);

			return result;
		}


		public async Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult, TRequest>(TRequest request, string? connectionString = null)
	where TRequest : SpRequest
	where TResult : class, DbResult, new()
		{
			if (connectionString.IsEmpty())
			{
				connectionString = ConnectionString;
			}

			var storedProcedure = $"[dbo].[{request.StoredProcedure}]";
			var result = new DataResult<IEnumerable<TResult>>();
			string executableString = string.Empty;

			try
			{
				// Create dynamic parameters from the request
				var dynamicParameters = CreateDynamicParameters(request);
				executableString = dynamicParameters.ExcecutableString;

				// Asynchronously retrieve all data
				var data = (await DapperManager.GetAllAsync<TResult>(storedProcedure, dynamicParameters.Parameters, connectionString))
						  ?? new List<TResult> { new() { bResult = false, vchMessage = "Sin resultados" } };

				result = DataResult<IEnumerable<TResult>>.Success(data);
			}
			catch (SqlException sqlEx)
			{
				_logger.LogError(sqlEx, "SQL error while executing stored procedure: {StoredProcedure} with {ExecutableString}", storedProcedure, executableString);
				result = DataResult<IEnumerable<TResult>>.Fail(sqlEx.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while executing stored procedure: {StoredProcedure} with {ExecutableString}", storedProcedure, executableString);
				result = DataResult<IEnumerable<TResult>>.Fail(ex.Message);
			}

			// Log and validate the operation's result
			LoggerHelper.ValidateResultLst(result);

			return result;
		}


		#endregion Gets from TRequest



		#region Gets from Objetc with List<TRequest>


		public async Task<DataResult<TResult>> GetWithListAsync<TResult, TRequest>(
			 TRequest request, string? connectionString = null
		)
			where TRequest : SpRequest
			where TResult : class, DbResult, new()
		{
			if (connectionString.IsEmpty())
			{
				connectionString = ConnectionString;
			}

			Console.WriteLine(BuildSqlCommand(request));

			DataResult<TResult> result;
			TResult responseObject = default(TResult);

			try
			{
				using (var connection = new SqlConnection(connectionString))
				{
					await connection.OpenAsync();

					using (var command = new SqlCommand(request.StoredProcedure, connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						// Add parameters from TRequest to the command
						AddParametersToCommand(command, request);

						// Execute the reader and map to TResult
						responseObject = await ExecuteReaderAndMapResults<TResult>(command);
					}
				}

				result = DataResult<TResult>.Success(responseObject);
			}
			catch (SqlException sqlEx)
			{
				// Use a descriptive message or have sqlCadena defined elsewhere
				result = DataResult<TResult>.Fail(Error.DBException(sqlEx.Number, "SQL query or description"), sqlEx);
			}
			catch (Exception ex)
			{
				result = DataResult<TResult>.Fail(Error.Exception("DPGA", ex.Message, "Additional Information"), ex);
			}

			LoggerHelper.ValidateResult(result);
			return result;

		}

		private static void AddParametersToCommand<TRequest>(SqlCommand command, TRequest request)
		{
			var properties = typeof(TRequest).GetProperties();

			foreach (var property in properties.Where(e => e.Name != "StoredProcedure"))
			{
				var propertyValue = property.GetValue(request);

				if (propertyValue is IEnumerable<object> list && property.PropertyType.IsGenericType)
				{
					// Get the generic type argument of the IEnumerable, which corresponds to your SQL DataType
					var genericType = property.PropertyType.GetGenericArguments()[0];
					var typeName = genericType.Name; // This should match the SQL type name if named correspondingly

					var listParameter = new SqlParameter
					{
						ParameterName = "@" + property.Name, // Ensures parameter naming convention
						SqlDbType = SqlDbType.Structured,
						TypeName = typeName,  // Dynamically assigned TypeName
						Value = ConvertToDataTable(list)
					};
					command.Parameters.Add(listParameter);
				}
				else
				{
					// Handle regular non-structured parameters
					command.Parameters.AddWithValue("@" + property.Name, propertyValue ?? DBNull.Value);
				}
			}
		}


		// Helper method to convert a list into a DataTable
		private static DataTable ConvertToDataTable(IEnumerable<object> list)
		{
			var dataTable = new DataTable();
			foreach (var item in list)
			{
				var itemType = item.GetType();
				var row = dataTable.NewRow();
				var properties = itemType.GetProperties();

				// Add columns if the DataTable is empty
				if (dataTable.Columns.Count == 0)
				{
					foreach (var property in properties)
					{
						dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
					}
				}

				// Fill the DataRow with the values
				foreach (var property in properties)
				{
					row[property.Name] = property.GetValue(item) ?? DBNull.Value;
				}

				dataTable.Rows.Add(row);
			}
			return dataTable;
		}

		// Helper method to execute reader and map to TResult
		private static async Task<TResult> ExecuteReaderAndMapResults<TResult>(SqlCommand command) where TResult : new()
		{
			TResult result = new TResult();

			using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow))
			{
				if (await reader.ReadAsync())
				{
					foreach (var property in typeof(TResult).GetProperties())
					{
						if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
						{
							var value = reader[property.Name];
							property.SetValue(result, value);
						}
					}
				}
			}

			return result;
		}

		public async Task<DataResult<IEnumerable<TResult>>> GetAllWithListAsync<TResult, TRequest>(
			 TRequest request, string? connectionString = null
		)
			where TRequest : SpRequest
			where TResult : class, DbResult, new()
		{
			if (connectionString.IsEmpty())
			{
				connectionString = ConnectionString;
			}

			Console.WriteLine(BuildSqlCommand(request));


			IEnumerable<TResult> responseObjects = Enumerable.Empty<TResult>();
			DataResult<IEnumerable<TResult>> result;

			try
			{
				using (var connection = new SqlConnection(connectionString))
				{
					await connection.OpenAsync();

					using (var command = new SqlCommand(request.StoredProcedure, connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						// Add parameters from TRequest to the command
						AddParametersToCommand(command, request);

						// Execute the reader and map results to a list
						responseObjects = await ExecuteReaderAndMapResultsToList<TResult>(command);
					}
				}

				result = DataResult<IEnumerable<TResult>>.Success(responseObjects);
			}
			catch (SqlException sqlEx)
			{
				result = DataResult<IEnumerable<TResult>>.Fail(Error.DBException(sqlEx.Number, "SQL query or description"), sqlEx);
			}
			catch (Exception ex)
			{
				result = DataResult<IEnumerable<TResult>>.Fail(Error.Exception("DPGA", ex.Message, "Additional Information"), ex);
			}

			return result;
		}

		private static async Task<IEnumerable<TResult>> ExecuteReaderAndMapResultsToList<TResult>(SqlCommand command)
	where TResult : new()
		{
			var results = new List<TResult>();

			using (var reader = await command.ExecuteReaderAsync())
			{
				while (await reader.ReadAsync())
				{
					var result = new TResult();
					foreach (var property in typeof(TResult).GetProperties())
					{
						if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
						{
							var value = reader[property.Name];
							property.SetValue(result, value);
						}
					}
					results.Add(result);
				}
			}

			return results;
		}



		#endregion Gets from Objetc with List<TRequest>


		#region Métodos extras 


		private DynamicParametersPair CreateDynamicParameters<T>(T obj) where T : SpRequest
		{
			var parameters = new DynamicParameters();
			var builder = new StringBuilder();

			builder.Append("EXEC").Append(' ').Append("dbo.").Append(obj.StoredProcedure).Append(' ');

			/**/

			var properties = obj.GetType().GetProperties();

			if (properties.NotEmpty())
			{

				var OnlyValidAtributos = properties.Where(e => e.Name != "StoredProcedure");
				if (OnlyValidAtributos.NotEmpty())
				{

					foreach (var prop in OnlyValidAtributos)
					{
						var dbType = DbTypeMap[prop.PropertyType.FullName];

						var value = prop.GetValue(obj);

						var property = prop.Name;
						parameters.Add(property, value, dbType);

						builder.Append("@").Append(property).Append(" = ").Append(Beautify(value)).Append(',');
					}

					builder.Remove(builder.ToString().LastIndexOf(","), 1);
				}
			}

			var excecutableString = builder.ToString();

			LoggerHelper.Info(excecutableString);

			return new DynamicParametersPair(parameters, excecutableString);
		}


		private object Beautify(object value)
		{
			if (value.NotNull())
			{
				var type = value.GetType();

				switch (type.FullName)
				{
					case "System.String":

						value = $"'{value}'";

						break;

					case "System.DateTime":

						value = $"'{((DateTime)value):yyyy-MM-dd}'";

						break;
				}
			}

			return value;
		}

		private sealed class DynamicParametersPair
		{
			public DynamicParameters Parameters { get; }

			public string ExcecutableString { get; }

			public DynamicParametersPair(DynamicParameters parameters, string excecutableString)
			{
				Parameters = parameters;
				ExcecutableString = excecutableString;
			}
		}

		private static string BuildSqlCommand<TRequest>(TRequest request)
		{
			var sqlBuilder = new StringBuilder();
			var properties = typeof(TRequest).GetProperties();

			foreach (var property in properties)
			{
				var propertyValue = property.GetValue(request);

				if (propertyValue is System.Collections.IEnumerable list && property.PropertyType.IsGenericType)
				{
					// Extract the generic type name (assuming it's the same as the SQL type)
					var parameterTypeName = property.PropertyType.GetGenericArguments()[0].Name;
					sqlBuilder.AppendLine($"DECLARE @{property.Name} {parameterTypeName};");

					foreach (var item in list)
					{
						var itemBuilder = new StringBuilder();
						itemBuilder.Append($"INSERT INTO @{property.Name} (");

						var itemProperties = item.GetType().GetProperties();
						foreach (var itemProperty in itemProperties)
						{
							itemBuilder.Append($"{itemProperty.Name}, ");
						}
						itemBuilder.Length -= 2;  // Remove last comma
						itemBuilder.Append(") VALUES (");

						foreach (var itemProperty in itemProperties)
						{
							var itemValue = itemProperty.GetValue(item);
							var formattedValue = FormatSqlValue(itemValue);
							itemBuilder.Append($"{formattedValue}, ");
						}
						itemBuilder.Length -= 2;  // Remove last comma
						itemBuilder.Append(");");

						sqlBuilder.AppendLine(itemBuilder.ToString());
					}
				}
				else
				{
					var formattedValue = FormatSqlValue(propertyValue);
					sqlBuilder.AppendLine($"-- For non-structured type parameter: {property.Name} = {formattedValue}");
				}
			}

			// Append final execution command
			sqlBuilder.AppendLine($"EXEC spGetTestTableDataType @{properties[0].Name} = @{properties[0].Name}");

			// Print the constructed SQL command
			Console.WriteLine(sqlBuilder.ToString());

			return sqlBuilder.ToString();
		}

		private static string FormatSqlValue(object value)
		{
			if (value == null || value == DBNull.Value)
				return "NULL";

			if (value is string || value is DateTime)
				return $"'{value}'";

			if (value is bool boolValue)
				return boolValue ? "1" : "0";

			return value.ToString();
		}



		#endregion Métodos extras 

	}
}
