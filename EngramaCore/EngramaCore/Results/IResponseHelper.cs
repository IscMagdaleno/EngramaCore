using EngramaCore.Dapper.Interfaces;

namespace EngramaCore.Results
{
	public interface IResponseHelper
	{
		Response<IEnumerable<TResultado>> Validacion<TProducto, TResultado>(IEnumerable<TProducto> ModeloValidar)
			where TProducto : DbResult;
		Response<TResultado> Validacion<TProducto, TResultado>(TProducto ModeloValidar)
			where TProducto : DbResult
			where TResultado : new();
		Response<TResultado> ValidacionSinMapper<TProducto, TResultado>(TProducto ModeloValidar)
			where TProducto : DbResult
			where TResultado : new();
		Response<IEnumerable<TResultado>> ValidacionSinMapper<TProducto, TResultado>(IEnumerable<TProducto> ModeloValidar)
			where TProducto : DbResult
			where TResultado : new();
	}
}