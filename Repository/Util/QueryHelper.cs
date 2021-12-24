using Domains;
using Repository.DTOs._Commom;
using Repository.DTOs.Tasks;
using System.Linq;

namespace Repository.Util
{
	public static class QueryHelper
	{
		public static string Like(this string text) => $"%{text}%";	
	}
} 