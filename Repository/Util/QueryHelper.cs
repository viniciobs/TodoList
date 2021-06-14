namespace Repository.Util
{
	public static class QueryHelper
	{
		public static string Like(this string text) => $"%{text}%";
	}
}