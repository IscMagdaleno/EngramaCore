using EngramaCore.Dapper.Interfaces;
using EngramaCore.Extensions;


using System.Runtime.CompilerServices;

namespace EngramaCore.Results
{
	public class ResultHelper : IResultHelper
	{
		private IMessageHandler MessageHandler { get; }

		public ResultHelper(IMessageHandler iMessageHandler)
		{
			MessageHandler = iMessageHandler;
		}

		public async Task<Result> ValidateAsync<T>(DataResult<T> dataResult,
												   bool notifyEmptyResult = false,
												   string emptyResultMessage = "",
												   bool notifyResult = true,
												   bool notifyError = true,
												   [CallerFilePath] string filePath = null,
												   [CallerMemberName] string method = null
		) where T : DbResult
		{
			var validateMessage = string.Empty;

			if (dataResult.Ok)
			{
				if (dataResult.Data.NotNull())
				{
					var validateErrorResult = ValidateError(dataResult.Data);
					validateMessage = validateErrorResult.Msg;

					if (validateErrorResult.Ok)
					{
						return Result.Success();
					}

					if (notifyResult && validateMessage.NotEmpty()) await MessageHandler.WarningAsync("ALERTA", validateMessage);
				}

				if (notifyEmptyResult)
				{
					validateMessage = emptyResultMessage;

					await MessageHandler.WarningAsync("ALERTA", validateMessage);
				}
			}
			else if (notifyError)
			{
				await MessageHandler.ErrorAsync("ERROR", dataResult.Error);
			}

			if (validateMessage.NotEmpty())
			{
				var callerType = Path.GetFileNameWithoutExtension(filePath);

				Console.WriteLine(string.Empty);
				Console.WriteLine($"{callerType}.{method}() => {validateMessage}");
				Console.WriteLine(string.Empty);
			}

			return Result.Fail(validateMessage);
		}

		public async Task<Result> ValidateAsync<T>(DataResult<IList<T>> dataResult,
												   bool notifyEmptyResult = false,
												   string emptyResultMessage = "",
												   bool notifyResult = true,
												   bool notifyError = true,
												   [CallerFilePath] string filePath = null,
												   [CallerMemberName] string method = null
		) where T : DbResult
		{
			var validateMessage = string.Empty;

			if (dataResult.Ok)
			{
				if (dataResult.Data.NotEmpty())
				{
					var validateErrorsResult = ValidateErrors(dataResult.Data);
					validateMessage = validateErrorsResult.Msg;

					if (validateErrorsResult.Ok)
					{
						return Result.Success();
					}

					if (notifyResult && validateMessage.NotEmpty())
					{
						await MessageHandler.WarningAsync("ALERTA", validateMessage);
					}
				}

				if (notifyEmptyResult)
				{
					validateMessage = emptyResultMessage;

					await MessageHandler.WarningAsync("ALERTA", validateMessage);
				}
			}
			else if (notifyError)
			{
				await MessageHandler.ErrorAsync("ERROR", dataResult.Error);
			}

			if (validateMessage.NotEmpty())
			{
				var callerType = Path.GetFileNameWithoutExtension(filePath);

				Console.WriteLine(string.Empty);
				Console.WriteLine($"{callerType}.{method}() => {validateMessage}");
				Console.WriteLine(string.Empty);
			}

			return Result.Fail(validateMessage);
		}

		private static Result ValidateErrors<T>(IEnumerable<T> data)
			 where T : DbResult
		{
			foreach (var item in data)
			{
				var validateErrorResult = ValidateError(item);

				if (validateErrorResult.Ok.False())
				{
					return Result.Fail(validateErrorResult.Msg);
				}
			}

			return Result.Success();
		}

		private static Result ValidateError<T>(T obj)
			 where T : DbResult
		{
			var resultado = obj.bResult;

			if (obj.bResult.False())
			{
				return Result.Fail(obj.vchMessage);

			}
			return Result.Success();


		}
	}
}
