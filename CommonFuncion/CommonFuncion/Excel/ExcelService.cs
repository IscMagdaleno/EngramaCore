using CommonFuncion.Results;

using OfficeOpenXml;

namespace EngramaCore.Excel
{
	public class ExcelService
	{
		public async Task<Response<List<TData>>> LoadExcelFile<TData>(Stream fileStream)
		where TData : new()
		{
			var response = new Response<List<TData>>();
			response.Data = new List<TData>();
			try
			{

				using (var memoryStream = new MemoryStream())
				{
					await fileStream.CopyToAsync(memoryStream);
					memoryStream.Position = 0;

					using (var package = new ExcelPackage(memoryStream))
					{
						var worksheet = package.Workbook.Worksheets[0];
						var rowCount = worksheet.Dimension.Rows;
						var columnCount = worksheet.Dimension.Columns;

						for (int row = 2; row <= rowCount; row++) // Assuming the first row is a header
						{
							var model = new TData();
							var properties = typeof(TData).GetProperties();

							for (int col = 1; col <= columnCount; col++)
							{
								var property = properties[col - 1];
								var cellValue = GetValueFromExcel(property.PropertyType, worksheet, row, col);

								property.SetValue(model, cellValue);
							}
							response.Data.Add(model);
						}

						response.IsSuccess = true;
						response.Message = "";
					}
				}

			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.Message = ex.Message;
			}
			return response;

		}

		private static object GetValueFromExcel(Type type, ExcelWorksheet worksheet, int row, int col)
		{
			object value;

			if (type == typeof(System.Int32))
			{
				value = worksheet.Cells[row, col].GetValue<int>();
			}
			else if (type == typeof(System.String))
			{
				value = worksheet.Cells[row, col].GetValue<string>() ?? string.Empty;
			}
			else if (type == typeof(System.DateTime))
			{
				value = worksheet.Cells[row, col].GetValue<DateTime>();
			}
			else if (type == typeof(System.Decimal))
			{
				value = worksheet.Cells[row, col].GetValue<decimal>();
			}
			else if (type == typeof(System.Double))
			{
				value = worksheet.Cells[row, col].GetValue<double>();
			}
			else if (type == typeof(System.Boolean))
			{
				value = worksheet.Cells[row, col].GetValue<bool>();
			}
			else
			{
				value = worksheet.Cells[row, col].Text; // Default to string if type is unknown
			}

			return value;
		}

	}
}
