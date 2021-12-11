using DataAccess;
using Domains.Exceptions;
using System.Threading.Tasks;

namespace Repository._Commom
{
	public abstract class Repository
	{
		#region Properties

		protected readonly ApplicationContext _db;

		#endregion Properties

		#region Constructor

		public Repository(ApplicationContext applicationContext)
		{
			if (applicationContext == null) throw new MissingArgumentsException(nameof(applicationContext));

			_db = applicationContext;
		}

		#endregion Constructor

		#region Methods

		public async Task SaveChangesAsync()
		{
			await _db.SaveChangesAsync();
		}

		#endregion Methods
	}
}