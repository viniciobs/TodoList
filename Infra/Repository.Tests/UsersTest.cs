using Domains;
using Domains.Exceptions;
using Repository.DTOs.Users;
using Repository.Interfaces;
using Repository.Tests.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Repository.Tests
{
    public class UsersTest(UserFixture userFixture) : IClassFixture<UserFixture>
    {
        private readonly Guid coreyId = userFixture.GetCoreyId();
        private readonly User adminUser = userFixture.GetAdminUser();
        private readonly User normalUser = userFixture.GetNormalUser();

        private readonly IUserRepository userRepository = userFixture.GetUserRepository();

        [Theory]
        [InlineData("Osbourne", "Kelly Osbourne", "Ozzy Osbourne")]
        [InlineData("gReEn", "Derrick Green")]
        [InlineData("Y", "Ozzy Osbourne", "Kelly Osbourne", "Corey Taylor")]
        public async Task FilterByName_ReturnExpectedCollection(string filterName, params string[] expectedNames)
        {
            var filter = new UserFilter { Name = filterName };
            var result = await userRepository.GetAsync(filter);
            var resultNames = result.Data.Select(x => x.Name).ToArray();

            Assert.Equivalent(resultNames, expectedNames);
        }

        [Fact]
        public async Task FilterUnexistingName_ReturnEmptyCollection()
        {
            var filter = new UserFilter { Name = "this is a name that does not exists" };
            var result = await userRepository.GetAsync(filter);
            var resultNames = result.Data.Select(x => x.Name).ToArray();

            Assert.Equal(resultNames, []);
        }

        [Theory]
        [InlineData("y", "kelly")]
        [InlineData("ar", "princeOfDarkness", "wrargh")]
        public async Task FilterByLogin_ReturnExpectedCollection(string filterLogin, params string[] expectedLogins)
        {
            var filter = new UserFilter { Login = filterLogin };
            var result = await userRepository.GetAsync(filter);
            var resultLogins = result.Data.Select(x => x.Login).ToArray();

            Assert.Equivalent(resultLogins, expectedLogins);
        }

        [Fact]
        public async Task FilterUnexistingLogin_ReturnEmptyCollection()
        {
            var filter = new UserFilter { Login = "this is a login that does not exists" };
            var result = await userRepository.GetAsync(filter);
            var resultLogins = result.Data.Select(x => x.Login).ToArray();

            Assert.Equal(resultLogins, []);
        }

        [Theory]
        [InlineData(true, "Ozzy Osbourne", "Kelly Osbourne", "Eddie Vedder", "Administrator", "Normal User")]
        [InlineData(false, "Corey Taylor", "Derrick Green")]
        [InlineData(null, "Ozzy Osbourne", "Kelly Osbourne", "Eddie Vedder", "Corey Taylor", "Derrick Green", "Administrator", "Normal User")]
        public async Task FilterByStatus_ReturnExpectedCollection(bool? isActive, params string[] expectedNames)
        {
            var filter = new UserFilter { IsActive = isActive, ItemsPerPage = 10 };
            var result = await userRepository.GetAsync(filter);
            var resultNames = result.Data.Select(x => x.Name).ToArray();

            Assert.Equivalent(resultNames, expectedNames);
        }

        [Fact]
        public async Task FindSingleUserWithInvalidFilter_ThrowMissingArgumentException()
        {
            await Assert.ThrowsAsync<MissingArgumentsException>(async () => await userRepository.FindAsync(default));
        }

        [Fact]
        public async Task AlterUserRoleWithUnexistingUser_ThrowNotFoundException()
        {
            var data = new AlterUserRoleData
            {
                TargetUser = adminUser.Id,
                AuthenticatedUser = Guid.NewGuid()
            };

            await Assert.ThrowsAsync<NotFoundException>(async () => await userRepository.AlterUserRoleAsync(data));

            data = new AlterUserRoleData
            {
                TargetUser = Guid.NewGuid(),
                AuthenticatedUser = adminUser.Id
            };

            await Assert.ThrowsAsync<NotFoundException>(async () => await userRepository.AlterUserRoleAsync(data));
        }

        [Fact]
        public async Task NormalUserAlterOtherUserRole_ThrowPermissionException()
        {            
            var data = new AlterUserRoleData
            {
                AuthenticatedUser = normalUser.Id,
                TargetUser = adminUser.Id
            };

            await Assert.ThrowsAsync<PermissionException>(async () => await userRepository.AlterUserRoleAsync(data));
        }

        [Fact]
        public async Task AdminAlterOtherUserRole_Ok()
        {
            var corey = await userRepository.FindAsync(coreyId);

            Assert.NotEqual(UserRole.Admin, corey.Role);

            var data = new AlterUserRoleData
            {
                AuthenticatedUser = adminUser.Id,
                TargetUser = coreyId,
                NewRole = UserRole.Admin
            };

            await userRepository.AlterUserRoleAsync(data);
            await userRepository.SaveChangesAsync();

            corey = await userRepository.FindAsync(coreyId);

            Assert.Equal(UserRole.Admin, corey.Role);
        }
    }
}