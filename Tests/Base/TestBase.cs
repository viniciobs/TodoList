using System;
using Task = Domains.User.Task;

namespace Domains.Tests
{
	public abstract class TestBase
	{
		// Arrange

		protected string NullComment => null;
		protected string EmptyComment => string.Empty;
		protected string ValidComment => "Change \"BLS\"s name";

		protected string NullName => null;
		protected string EmptyName => string.Empty;
		protected string ValidName => "Ozzy Osbourne";

		protected string NullLogin => null;
		protected string EmptyLogin => string.Empty;
		protected string ValidLogin => "prince.of.darkness";

		protected string NullPassword => null;
		protected string EmptyPassword => string.Empty;
		protected string ValidPassword => "0224Osb0urn*";

		protected string NullTaskDescription => null;
		protected string EmptyTaskDescription => string.Empty;
		protected string ValidTaskDescription => "To learn how to build nice APIs";

		protected User NullUser => null;
		protected Task NullTask => null;

		protected User GenerateRandomUser() => User.New(GenerateRandomString(), GenerateRandomString());

		protected User GenerateAdminUser() => User.NewAdmin();

		protected Task GenerateRandomTask() => GenerateRandomUser().SetTask(GenerateRandomUser(), ValidTaskDescription);

		protected string GenerateRandomString() => Guid.NewGuid().ToString("N").ToLower().Substring(0, 10);
	}
}