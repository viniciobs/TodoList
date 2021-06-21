using System.Threading.Tasks;

namespace Repository.Interfaces.Base
{
	public interface IRepository
	{
		public Task SaveChangesAsync();
	}
}