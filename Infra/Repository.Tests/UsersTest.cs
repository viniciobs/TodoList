using Domains;
using Domains.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.DTOs.Accounts;
using Repository.DTOs.Users;
using Repository.Tests.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Tests
{
    [TestClass]
    public class UsersTest : RepositoryTestBase
    {
        private readonly Guid coreyId;
        private readonly Guid derrickId;

        public UsersTest()
        {
            var password = GenerateRandomString();

            adminUser.Activate();
            context.User.Add(adminUser);

            accountRepository.CreateAsync(new CreateAccountData { Login = "princeOfDarkness", Name = "Ozzy Osbourne", Password = password }).Wait();
            accountRepository.CreateAsync(new CreateAccountData { Login = "kelly", Name = "Kelly Osbourne", Password = password }).Wait();
            accountRepository.CreateAsync(new CreateAccountData { Login = "niceVoice", Name = "Eddie Vedder", Password = GenerateRandomString() }).Wait();

            coreyId = accountRepository.CreateAsync(new CreateAccountData { Login = "cmft", Name = "Corey Taylor", Password = GenerateRandomString() }).Result;
            derrickId = accountRepository.CreateAsync(new CreateAccountData { Login = "wrargh", Name = "Derrick Green", Password = GenerateRandomString() }).Result;

            accountRepository.SaveChangesAsync().Wait();

            accountRepository.AlterStatusAsync(coreyId, false).Wait();
            accountRepository.AlterStatusAsync(derrickId, false).Wait();

            accountRepository.SaveChangesAsync().Wait();
        }

        [TestMethod]
        [DataRow("Osbourne", "Kelly Osbourne", "Ozzy Osbourne")]
        [DataRow("gReEn", "Derrick Green")]
        [DataRow("Y", "Ozzy Osbourne", "Kelly Osbourne", "Corey Taylor")]
        public async Task FilterByName_ReturnExpectedCollection(string filterName, params string[] expectedNames)
        {
            var filter = new UserFilter { Name = filterName };
            var result = await userRepository.GetAsync(filter);
            var resultNames = result.Data.Select(x => x.Name).ToArray();

            CollectionAssert.AreEquivalent(resultNames, expectedNames);
        }

        [TestMethod]
        public async Task FilterUnexistingName_ReturnEmptyCollection()
        {
            var filter = new UserFilter { Name = "this is a name that does not exists" };
            var result = await userRepository.GetAsync(filter);
            var resultNames = result.Data.Select(x => x.Name).ToArray();

            CollectionAssert.AreEqual(resultNames, Array.Empty<string>());
        }

        [TestMethod]
        [DataRow("y", "kelly")]
        [DataRow("ar", "princeOfDarkness", "wrargh")]
        public async Task FilterByLogin_ReturnExpectedCollection(string filterLogin, params string[] expectedLogins)
        {
            var filter = new UserFilter { Login = filterLogin };
            var result = await userRepository.GetAsync(filter);
            var resultLogins = result.Data.Select(x => x.Login).ToArray();

            CollectionAssert.AreEquivalent(resultLogins, expectedLogins);
        }

        [TestMethod]
        public async Task FilterUnexistingLogin_ReturnEmptyCollection()
        {
            var filter = new UserFilter { Login = "this is a login that does not exists" };
            var result = await userRepository.GetAsync(filter);
            var resultLogins = result.Data.Select(x => x.Login).ToArray();

            CollectionAssert.AreEqual(resultLogins, Array.Empty<string>());
        }

        [TestMethod]
        [DataRow(true, "Ozzy Osbourne", "Kelly Osbourne", "Eddie Vedder", "Administrator")]
        [DataRow(false, "Corey Taylor", "Derrick Green")]
        [DataRow(null, "Ozzy Osbourne", "Kelly Osbourne", "Eddie Vedder", "Corey Taylor", "Derrick Green", "Administrator")]
        public async Task FilterByStatus_ReturnExpectedCollection(bool? isActive, params string[] expectedNames)
        {
            var filter = new UserFilter { IsActive = isActive, ItemsPerPage = 10 };
            var result = await userRepository.GetAsync(filter);
            var resultNames = result.Data.Select(x => x.Name).ToArray();

            CollectionAssert.AreEquivalent(resultNames, expectedNames);
        }

        [TestMethod]
        public async Task FindSingleUserWithInvalidFilter_ThrowMissingArgumentException()
        {
            Assert.ThrowsExceptionAsync<MissingArgumentsException>(async () => await userRepository.FindAsync(default));
        }

        [TestMethod]
        public async Task AlterUserRoleWithUnexistingUser_ThrowNotFoundException()
        {
            var data = new AlterUserRoleData
            {
                TargetUser = adminUser.Id,
                AuthenticatedUser = Guid.NewGuid()
            };

            Assert.ThrowsExceptionAsync<NotFoundException>(async () => await userRepository.AlterUserRoleAsync(data));

            data = new AlterUserRoleData
            {
                TargetUser = Guid.NewGuid(),
                AuthenticatedUser = adminUser.Id
            };

            Assert.ThrowsExceptionAsync<NotFoundException>(async () => await userRepository.AlterUserRoleAsync(data));
        }

        [TestMethod]
        public async Task NormalUserAlterOtherUserRole_ThrowPermissionException()
        {
            var data = new AlterUserRoleData
            {
                AuthenticatedUser = normalUser.Id,
                TargetUser = adminUser.Id
            };

            Assert.ThrowsExceptionAsync<PermissionException>(async () => await userRepository.AlterUserRoleAsync(data));
        }

        [TestMethod]
        public async Task AdminAlterOtherUserRole_Ok()
        {
            var corey = await userRepository.FindAsync(coreyId);

            Assert.AreNotEqual(corey.Role, UserRole.Admin);

            var data = new AlterUserRoleData
            {
                AuthenticatedUser = adminUser.Id,
                TargetUser = coreyId,
                NewRole = UserRole.Admin
            };

            await userRepository.AlterUserRoleAsync(data);
            await userRepository.SaveChangesAsync();

            corey = await userRepository.FindAsync(coreyId);

            Assert.AreEqual(corey.Role, UserRole.Admin);
        }
    }
}