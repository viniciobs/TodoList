using System.Threading.Tasks;

namespace Repository.Interfaces
{
	public interface IRepository
	{
		public Task SaveChangesAsync();
	}
}