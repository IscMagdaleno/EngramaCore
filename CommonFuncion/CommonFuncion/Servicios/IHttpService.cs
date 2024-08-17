namespace CommonFuncion.Servicios
{
	public interface IHttpService
	{
		Task<HttpResponseWrapper<T>> Get<T>(string url);
		Task<HttpResponseWrapper<object>> Post<T>(string url, T data);
		Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T data);
		Task<HttpResponseWrapper<TResponse>> Post<TResponse>(string url);
		Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T data, Dictionary<string, string> headers = null);
		Task<HttpResponseWrapper<TResponse>> PostWithFile<TResponse>(string url, StreamContent fileContent, Dictionary<string, string> headers = null);
		Task<HttpResponseWrapper<TResponse>> PostDataContentVideo<TResponse>(string url, StreamContent? fileContent, string name);
		Task<HttpResponseWrapper<TResponse>> PostDataContentImage<TResponse>(string url, StreamContent? fileContent, string name);
	}
}



