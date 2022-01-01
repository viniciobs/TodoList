using Domains.Tests;
using Repository.DTOs.Accounts;

namespace Repository.Tests.Base
{
	public abstract class RepositoryTestBase : DomainTestBase
	{
		// Arrange

		protected CreateAccountData GetValidCreateAccountData()
		{
			var randomUser = GenerateRandomUser();
			
			var randomValidAccountData = new CreateAccountData()
			{
				Name = randomUser.Name,
				Login = randomUser.Login,
				Password = GenerateRandomString()
			};

			return randomValidAccountData;
		}


	}
}
