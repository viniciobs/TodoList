using DataAccess;
using Domains.Exceptions;
using System.Threading.Tasks;

namespace Repository.Base
{
    public abstract class Repository(ApplicationContext applicationContext)
    {
        protected readonly ApplicationContext _db = applicationContext ?? throw new MissingArgumentsException(nameof(applicationContext));

        public Task SaveChangesAsync() =>
            _db.SaveChangesAsync();        
    }
}