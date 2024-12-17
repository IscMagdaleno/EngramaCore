namespace EngramaCore.Extensions
{
	public static class BoolExt
	{
		public static bool False(this bool @bool)
		{
			return !@bool;
		}
	}
}
