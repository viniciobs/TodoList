using DataAccess;
using System;
using System.Threading.Tasks;

namespace Repository
{
	public abstract class Repository
	{
		#region Properties

		protected readonly ApplicationContext _db;

		#endregion Properties

		#region Constructor

		public Repository(ApplicationContext applicationContext)
		{
			if (applicationContext == null) throw new ArgumentNullException(nameof(applicationContext));

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