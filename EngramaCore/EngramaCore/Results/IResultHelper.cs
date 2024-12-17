using EngramaCore.Dapper.Interfaces;

using System.Runtime.CompilerServices;

namespace EngramaCore.Results
{
	public interface IResultHelper
	{
		Task<Result> ValidateAsync<T>(DataResult<T> dataResult, bool notifyEmptyResult = false, string emptyResultMessage = "", bool notifyResult = true, bool notifyError = true, [CallerFilePath] string filePath = null, [CallerMemberName] string method = null) where T : DbResult;
		Task<Result> ValidateAsync<T>(DataResult<IList<T>> dataResult, bool notifyEmptyResult = false, string emptyResultMessage = "", bool notifyResult = true, bool notifyError = true, [CallerFilePath] string filePath = null, [CallerMemberName] string method = null) where T : DbResult;
	}
}
