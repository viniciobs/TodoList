using DataAccess;
using Domains.Exceptions;
using System.Threading.Tasks;

namespace Repository.Base
{
    public abstract class Repository
    {
        protected readonly ApplicationContext _db;

        public Repository(ApplicationContext applicationContext)
        {
            _db = applicationContext ?? throw new MissingArgumentsException(nameof(applicationContext));
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}