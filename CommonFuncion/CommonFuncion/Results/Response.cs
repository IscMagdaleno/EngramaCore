namespace CommonFuncion.Results
{
	public class Response<T>
	{
		public T Data { get; set; }

		public bool IsSuccess { get; set; }

		public string Message { get; set; } = string.Empty;


		public static Response<T> BadResult(string Message, T data)
		{
			var result = new Response<T>();
			result.IsSuccess = false;
			result.Message = Message;
			result.Data = data;

			return result;
		}
	}
}
