namespace CommonFuncion.Identity
{

	public interface ILoginService
	{
		Task LogIn(string token);
		Task LogOut();
	}

}
