using CommonFuncion.Dapper.Interfaces;
using CommonFuncion.Extensions;
using CommonFuncion.Logger;
using CommonFuncion.Results;

using Dapper;

using Microsoft.Extensions.Configuration;

using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CommonFuncion.Dapper
{
	/// <summary>
	/// Auxiliar para el uso de dapper al convertir los modelos ya en la lista de parámetros 
	/// </summary>
	public class DapperMangerHelper : IDapperManagerHelper
	{
		public IDapperManager DapperManager { get; set; }
		public ILoggerHelper LoggerHelper { get; }

		/**/

		private readonly Dictionary<string, DbType> DbTypeMap = new();

		/**/

		private string ConnectionString { get; set; }


		public DapperMangerHelper(IDapperManager dapperManager, ILoggerHelper loggerHelper, IConfiguration configuration)
		{
			ConnectionString = configuration.GetConnectionString("EngramaCloudConnection");

			DapperManager = dapperManager;
			LoggerHelper = loggerHelper;
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

		public async Task<DataResult<TResult>> GetAsync<TResult>(string Script, string conectionString = null)
			where TResult : class, DbResult, new()
		{
			var resp = new DataResult<TResult>();
			try
			{
				var data = DapperManager.Get<TResult>(Script, conectionString);
				if (data.IsNull())
				{
					data = (new() { bResult = false, vchMessage = "Sin resultados" });
				}

				LoggerHelper.ValidateSPResult(data);

				resp = await Task.FromResult(DataResult<TResult>.Success(data));
			}
			catch (Exception ex)
			{

				resp = await Task.FromResult(DataResult<TResult>.Fail(ex.Message));

			}

			LoggerHelper.ValidateResult(resp);
			return resp;
		}

		public async Task<DataResult<IList<TResult>>> GetAllAsync<TResult>(string Script, string conectionString = null)
			where TResult : class, DbResult, new()
		{
			var resp = new DataResult<IList<TResult>>();

			try
			{
				var data = DapperManager.GetAll<TResult>(Script, conectionString);

				if (data.IsEmpty())
				{
					data.Add(new() { bResult = false, vchMessage = "Sin resultados" });
				}

				resp = await Task.FromResult(DataResult<IList<TResult>>.Success(data));
			}
			catch (Exception ex)
			{

				resp = await Task.FromResult(DataResult<IList<TResult>>.Fail(ex.Message));
			}

			//LoggerHelper.ValidateResult(resp);
			return resp;

		}

		#endregion Gets from script


		#region Gets from TRequest
		public async Task<DataResult<TResult>> GetAsync<TResult, TRequest>(TRequest request, string? conectionString = null)
			where TRequest : SpRequest
			where TResult : class, DbResult, new()
		{

			var result = new DataResult<TResult>();
			var sp = $"[dbo].[{request.StoredProcedure}]";

			var executableString = string.Empty;

			try
			{
				var dynamicParameters = CreateDynamicParameters(request);

				executableString = dynamicParameters.ExcecutableString;

				var data = DapperManager.Get<TResult>(sp, dynamicParameters.Parameters, conectionString);
				if (data.IsNull())
				{
					data = (new() { bResult = false, vchMessage = "Sin resultados" });
				}

				result = await Task.FromResult(DataResult<TResult>.Success(data));
			}
			catch (Exception ex)
			{
				if (ex is SqlException sqlEx)
				{
					result = await Task.FromResult(DataResult<TResult>.Fail(executableString, sqlEx));
				}
				else
				{
					result = await Task.FromResult(DataResult<TResult>.Fail(executableString));
				}
			}

			LoggerHelper.ValidateResult(result);

			return result;
		}

		public async Task<DataResult<IList<TResult>>> GetAllAsync<TResult, TRequest>(TRequest request, string? conectionString = null)
			where TRequest : SpRequest
			where TResult : class, DbResult, new()
		{
			var sp = $"[dbo].[{request.StoredProcedure}]";
			var result = new DataResult<IList<TResult>>();

			var executableString = string.Empty;

			try
			{
				var dynamicParameters = CreateDynamicParameters(request);

				executableString = dynamicParameters.ExcecutableString;

				var data = DapperManager.GetAll<TResult>(sp, dynamicParameters.Parameters, conectionString);
				if (data.IsNull())
				{
					data = new List<TResult> { new() { bResult = false, vchMessage = "Sin resultados" } };
				}

				result = await Task.FromResult(DataResult<IList<TResult>>.Success(data));
			}
			catch (Exception ex)
			{
				result = await Task.FromResult(DataResult<IList<TResult>>.Fail(ex.Message));
			}

			//LoggerHelper.ValidateResult(result);

			return result;
		}

		#endregion Gets from TRequest


		#region Gets from List<TRequest>

		public async Task<DataResult<TResult>> GetFromListAsync<TResult, TRequest>(
			string connectionString, string storeProcedure, string TypeName, IList<TRequest> model
		)
		where TRequest : SpRequest
		where TResult : class, DbResult, new()
		{
			var result = new TResult();
			var query = $"DECLARE @varOpciones {TypeName} INSERT INTO @varOpciones VALUES";

			await using (var connection = new SqlConnection(connectionString))
			{
				var dictionaries = model.Select(x => x.ToDictionary<string, object>()).ToList();

				using (var dataTable = new DataTable())
				{

					foreach (var item in dictionaries.FirstOrDefault())
					{
						dataTable.Columns.Add(item.Key);
						dataTable.Columns[item.Key].DataType = item.Value.GetType();
					}

					foreach (var opcionesItems in dictionaries)
					{
						query += "(";
						var row = dataTable.NewRow();
						foreach (var item in opcionesItems)
						{
							query += $"{Beautify(item.Value)},";

							row[item.Key] = item.Value;

						}

						dataTable.Rows.Add(row);
						query += "),";
						query = query.Replace(",),", "),");
					}

					query += ";";
					query = query.Replace(",;", ";");


					try
					{
						connection.Open();

						if (connection.State == ConnectionState.Open)
						{
							var sqlCadena = $"EXEC dbo.{storeProcedure} @varOpciones ";
							var command = new SqlCommand(sqlCadena, connection);

							var tvpParam = command.Parameters.AddWithValue("@varOpciones", dataTable);
							tvpParam.SqlDbType = SqlDbType.Structured;
							tvpParam.TypeName = $"dbo.{TypeName}";

							var exec = command.CommandText;

							foreach (SqlParameter param in command.Parameters)
							{
								exec = exec.Replace(param.ParameterName, $"{param.ParameterName} = {(param.Value.ToString().NotEmpty() ? param.Value : "''")}");
							}

							var info = new StringBuilder();
							var sp = exec.Replace(", ", ",\n\t\t");

							info.Append("\n\n\t" + sp + "\n");


							await using (var executeReader = await command.ExecuteReaderAsync())
							{
								while (executeReader.Read())
								{
									var tmpmodel = new TResult();
									var properties = tmpmodel.GetType().GetProperties();
									var i = 0;

									foreach (var property in properties)
									{
										var value = executeReader.GetValue(i);
										tmpmodel.GetType().GetProperty(property.Name).SetValue(tmpmodel, value);
										i++;
									}

									result = (tmpmodel);
								}
							}

							connection.Close();
						}
					}
					catch (Exception ex)
					{
						/**/
					}
				}
			}

			var resultado = await Task.FromResult(DataResult<TResult>.Success(result));
			//LoggerHelper.ValidateResult(resultado);

			return resultado;
		}

		public async Task<DataResult<IList<TResult>>> GetAllFromListAsync<TResult, TRequest>(
			string connectionString, string storeProcedure, string TypeName, IList<TRequest> model
		)
			where TRequest : SpRequest
			where TResult : class, DbResult, new()
		{
			var result = new List<TResult>();

			var query = $"DECLARE @varOpciones {TypeName} INSERT INTO @varOpciones VALUES";

			await using (var connection = new SqlConnection(connectionString))
			{
				var dictionaries = model.Select(x => x.ToDictionary<string, object>()).ToList();

				using (var dataTable = new DataTable())
				{

					foreach (var item in dictionaries.FirstOrDefault())
					{
						dataTable.Columns.Add(item.Key);
						dataTable.Columns[item.Key].DataType = item.Value.GetType();
					}

					foreach (var opcionesItems in dictionaries)
					{
						query += "(";

						var row = dataTable.NewRow();
						foreach (var item in opcionesItems)
						{
							query += $"{Beautify(item.Value)},";

							row[item.Key] = item.Value;
						}

						dataTable.Rows.Add(row);
						query += "),";
						query = query.Replace(",),", "),");
					}
					query += ";";
					query = query.Replace(",;", ";");


					try
					{
						connection.Open();

						if (connection.State == ConnectionState.Open)
						{
							var sqlCadena = $"EXEC dbo.{storeProcedure} @varOpciones ";
							var command = new SqlCommand(sqlCadena, connection);

							var tvpParam = command.Parameters.AddWithValue("@varOpciones", dataTable);
							tvpParam.SqlDbType = SqlDbType.Structured;
							tvpParam.TypeName = $"dbo.{TypeName}";

							var exec = command.CommandText;

							foreach (SqlParameter param in command.Parameters)
							{
								exec = exec.Replace(param.ParameterName, $"{param.ParameterName} = {(param.Value.ToString().NotEmpty() ? param.Value : "''")}");
							}

							var info = new StringBuilder();
							var sp = exec.Replace(", ", ",\n\t\t");

							info.Append("\n\n\t" + sp + "\n");


							await using (var executeReader = await command.ExecuteReaderAsync())
							{
								while (executeReader.Read())
								{
									var tmpmodel = new TResult();
									var properties = tmpmodel.GetType().GetProperties();
									var i = 0;

									foreach (var property in properties)
									{
										var value = executeReader.GetValue(i);
										tmpmodel.GetType().GetProperty(property.Name).SetValue(tmpmodel, value);
										i++;
									}

									result.Add(tmpmodel);
								}
							}

							connection.Close();
						}
					}
					catch (Exception ex)
					{
						/**/
					}
				}
			}

			var resultado = await Task.FromResult(DataResult<IList<TResult>>.Success(result));
			//LoggerHelper.ValidateResult(resultado);

			return resultado;
		}

		#endregion Gets from List<TRequest>


		#region Gets from Objetc with List<TRequest>

		public async Task<DataResult<TResult>> GetCombinedAsync<TResult, TRequest>(
			 TRequest request, string? connectionString = null
		)
			where TRequest : SpRequest
			where TResult : class, DbResult, new()
		{
			if (connectionString.IsEmpty())
			{
				connectionString = ConnectionString;
			}

			var sqlCadena = "EXEC " + request.StoredProcedure;
			var query = string.Empty;

			TResult respuesta = new TResult();
			string TypeStructured = "";
			var space = " ";

			var result = new DataResult<TResult>();

			using (var connection = new SqlConnection(connectionString))
			{
				using (var command = new SqlCommand(request.StoredProcedure, connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					using (var dataTable = new DataTable())
					{
						try
						{
							connection.Open();

							if (connection.State == ConnectionState.Open)
							{
								var properties = request.GetType().GetProperties();

								var OnlyValidAtributos = properties.Where(e => e.Name != "StoredProcedure");

								foreach (var prop in OnlyValidAtributos)
								{
									var pair = AnalyzePropertyForList(prop.PropertyType, prop.GetValue(request));
									if (pair.Item2 == SqlDbType.Structured)
									{
										var lst = (pair.Item1 as IEnumerable<object>).Cast<object>().ToList();
										if (lst.Count > 0)
											sqlCadena = sqlCadena + " @" + prop.Name + "," + space;
									}
									else
										sqlCadena = sqlCadena + " @" + prop.Name + "," + space;
								}

								sqlCadena = sqlCadena.Remove(sqlCadena.Length - 2, 2);

								foreach (var prop in OnlyValidAtributos)
								{
									var pair = AnalyzePropertyForList(prop.PropertyType, prop.GetValue(request));

									if (pair.Item2 == SqlDbType.Structured)
									{
										var lst = (pair.Item1 as IEnumerable<object>).Cast<object>().ToList();

										if (lst.Count > 0)
										{
											var dictionaries = lst.Select(x => x.ToDictionary<string, object>()).ToList();
											TypeStructured = TypeStructured + Jsons.Stringify(lst) + "\n\n";

											string TypeName = lst.FirstOrDefault().GetType().Name;
											query = $"DECLARE @varOpciones_{prop.Name} {TypeName} INSERT INTO @varOpciones_{prop.Name} VALUES";
											foreach (var item in dictionaries.FirstOrDefault())
											{
												dataTable.Columns.Add(item.Key);
												dataTable.Columns[item.Key].DataType = item.Value.GetType();
											}

											foreach (var opcionesItems in dictionaries)
											{
												var row = dataTable.NewRow();
												query += "(";
												foreach (var item in opcionesItems)
												{
													row[item.Key] = item.Value;
													query += $"{Beautify(item.Value)},";
												}
												dataTable.Rows.Add(row);
												query += "),";
												query = query.Replace(",),", "),");
											}
											query += ";";
											query = query.Replace(",;", ";");

											var tvpParam = command.Parameters.AddWithValue("@" + prop.Name + "", dataTable);
											tvpParam.SqlDbType = pair.Item2;
											tvpParam.TypeName = TypeName;
										}
									}
									else
									{
										var tvpParam = command.Parameters.AddWithValue("@" + prop.Name + "", pair.Item1);
										tvpParam.SqlDbType = pair.Item2;
									}
								}

								var exec = command.CommandText;

								foreach (SqlParameter param in command.Parameters)
								{
									if (param.DbType == DbType.Object)
									{
										var opsList = "@varOpciones_" + param.ParameterName.Replace("@", "");
										sqlCadena = sqlCadena.Replace(param.ParameterName, $"{param.ParameterName} = {opsList}");
									}
									else
										sqlCadena = sqlCadena.Replace(param.ParameterName, $"{param.ParameterName} = {(param.Value.ToString().NotEmpty() ? (param.SqlDbType == SqlDbType.Int ? param.Value : "'" + (param.SqlDbType == SqlDbType.DateTime ? param.Value.ToDateTime().ToString("MM/dd/yyyy HH:mm:ss") : param.Value) + "'") : "''")}");
								}

								sqlCadena = "\n" + sqlCadena.Replace(", ", ",\n\t\t");


								using (var executeReader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow))
								{
									while (executeReader.Read())
									{
										var tmpmodel = new TResult();
										var propertie = tmpmodel.GetType().GetProperties();
										var i = 0;

										foreach (var property in propertie)
										{
											var value = executeReader.GetValue(i);
											tmpmodel.GetType().GetProperty(property.Name).SetValue(tmpmodel, value);
											i++;
										}

										respuesta = (tmpmodel);
									}
								}
							}
						}
						catch (Exception ex)
						{

							if (ex is SqlException sqlEx)
							{
								result = await Task.FromResult(DataResult<TResult>.Fail(Error.DBException(sqlEx.Number, sqlCadena), sqlEx));
							}
							else
							{

								result = await Task.FromResult(DataResult<TResult>.Fail(Error.Exception("DPGA", ex.Message, sqlCadena + " " + TypeStructured)));
							}
						}

						result = await Task.FromResult(DataResult<TResult>.Success(respuesta));
					}
				}
			}

			LoggerHelper.ValidateResult(result);
			return result;

		}

		public async Task<DataResult<IList<TResult>>> GetAllCombinedAsync<TResult, TRequest>(
			string connectionString, TRequest request
		)
			where TRequest : SpRequest
			where TResult : class, DbResult, new()
		{
			var sqlCadena = "EXEC " + request.StoredProcedure;
			List<TResult> respuesta = new List<TResult>();
			string TypeStructured = "";
			var space = " ";
			var query = string.Empty;
			var result = new DataResult<IList<TResult>>();

			using (var connection = new SqlConnection(connectionString))
			{
				using (var command = new SqlCommand(request.StoredProcedure, connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					using (var dataTable = new DataTable())
					{
						try
						{
							connection.Open();

							if (connection.State == ConnectionState.Open)
							{
								foreach (var prop in request.GetType().GetProperties())
								{
									var pair = AnalyzePropertyForList(prop.PropertyType, prop.GetValue(request));
									if (pair.Item2 == SqlDbType.Structured)
									{
										var lst = (pair.Item1 as IEnumerable<object>).Cast<object>().ToList();
										if (lst.Count > 0)
											sqlCadena = sqlCadena + " @" + prop.Name + "," + space;
									}
									else
										sqlCadena = sqlCadena + " @" + prop.Name + "," + space;
								}

								sqlCadena = sqlCadena.Remove(sqlCadena.Length - 2, 2);

								foreach (var prop in request.GetType().GetProperties())
								{
									var pair = AnalyzePropertyForList(prop.PropertyType, prop.GetValue(request));

									if (pair.Item2 == SqlDbType.Structured)
									{
										var lst = (pair.Item1 as IEnumerable<object>).Cast<object>().ToList();
										if (lst.Count > 0)
										{
											var dictionaries = lst.Select(x => x.ToDictionary<string, object>()).ToList();
											TypeStructured = TypeStructured + Jsons.Stringify(lst) + "\n\n";

											string TypeName = lst.FirstOrDefault().GetType().Name;
											query = $"DECLARE @varOpciones_{prop.Name} {TypeName} INSERT INTO @varOpciones_{prop.Name} VALUES";
											foreach (var item in dictionaries.FirstOrDefault())
											{
												dataTable.Columns.Add(item.Key);
												dataTable.Columns[item.Key].DataType = item.Value.GetType();
											}

											foreach (var opcionesItems in dictionaries)
											{
												var row = dataTable.NewRow();
												query += "(";
												foreach (var item in opcionesItems)
												{
													query += $"{Beautify(item.Value)},";
													row[item.Key] = item.Value;
												}
												dataTable.Rows.Add(row);
												query += "),";
												query = query.Replace(",),", "),");
											}
											query += ";";
											query = query.Replace(",;", ";");

											var tvpParam = command.Parameters.AddWithValue("@" + prop.Name + "", dataTable);
											tvpParam.SqlDbType = pair.Item2;
											tvpParam.TypeName = TypeName;
										}
									}
									else
									{
										var tvpParam = command.Parameters.AddWithValue("@" + prop.Name + "", pair.Item1);
										tvpParam.SqlDbType = pair.Item2;
									}
								}

								var exec = command.CommandText;

								foreach (SqlParameter param in command.Parameters)
								{
									if (param.DbType == DbType.Object)
									{
										var opsList = "@varOpciones_" + param.ParameterName.Replace("@", "");
										sqlCadena = sqlCadena.Replace(param.ParameterName, $"{param.ParameterName} = {opsList}");
									}
									else
										sqlCadena = sqlCadena.Replace(param.ParameterName, $"{param.ParameterName} = {(param.Value.ToString().NotEmpty() ? (param.SqlDbType == SqlDbType.Int ? param.Value : "'" + (param.SqlDbType == SqlDbType.DateTime ? param.Value.ToDateTime().ToString("MM/dd/yyyy HH:mm:ss") : param.Value) + "'") : "''")}");
								}

								sqlCadena = "\n" + sqlCadena.Replace(", ", ",\n\t\t");


								using (var executeReader = await command.ExecuteReaderAsync())
								{
									while (executeReader.Read())
									{
										var tmpmodel = new TResult();
										var properties = tmpmodel.GetType().GetProperties();
										var i = 0;

										foreach (var property in properties)
										{
											var value = executeReader.GetValue(i);
											tmpmodel.GetType()
												.GetProperty(property.Name)
												.SetValue(tmpmodel, value);
											i++;
										}

										respuesta.Add(tmpmodel);
									}
								}
							}
						}
						catch (Exception ex)
						{

							if (ex is SqlException sqlEx)
							{
								result = await Task.FromResult(DataResult<IList<TResult>>.Fail(Error.DBException(sqlEx.Number, sqlCadena), sqlEx));
							}
							else
							{

								result = await Task.FromResult(DataResult<IList<TResult>>.Fail(Error.Exception("DPGA", ex.Message, sqlCadena)));
							}
						}

						result = await Task.FromResult(DataResult<IList<TResult>>.Success(respuesta));
					}
				}
			}

			//LoggerHelper.ValidateResult(result);
			return result;

		}

		#endregion Gets from Objetc with List<TRequest>


		#region Métodos extras 

		private static Tuple<object, SqlDbType> AnalyzePropertyForList(Type type, object value)
		{
			var dbType = SqlDbType.Variant;

			switch (type.FullName)
			{
				case "System.Boolean":

					dbType = SqlDbType.Bit;

					break;

				case "System.Byte":

					dbType = SqlDbType.Bit;

					break;

				case "System.Byte[]":

					dbType = SqlDbType.Binary;

					break;

				case "System.Int32":

					dbType = SqlDbType.Int;

					break;

				case "System.String":

					dbType = SqlDbType.VarChar;
					value ??= string.Empty;

					break;

				case "System.DateTime":

					dbType = SqlDbType.DateTime;
					value ??= Defaults.SqlMinDate();

					break;

				case "System.Decimal":

					dbType = SqlDbType.Decimal;

					break;

				case "System.Single":

					dbType = SqlDbType.Float;

					break;

				case "System.Int64":

					dbType = SqlDbType.BigInt;

					break;
				default:
					dbType = SqlDbType.Structured;
					break;

			}

			return new Tuple<object, SqlDbType>(value, dbType);
		}

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

		#endregion Métodos extras 

	}
}
