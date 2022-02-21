using Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs.Accounts;
using Repository.DTOs.History;
using Repository.Interfaces;
using System;
using System.Threading.Tasks;
using ToDoList.API.Services.TokenGenerator.Interfaces;
using ToDoList.API.Services.TokenGenerator.Models;
using ToDoList.UI.Controllers.Commom;

namespace ToDoList.UI.Controllers
{
    /// <summary>
    /// Responsible class for account management
    /// </summary>
    [Produces("application/json")]
    [Route("accounts")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _repo;
        private readonly IHistoryRepository _historyRepo;

        public AccountsController(IAccountRepository repo, IHistoryRepository historyRepo)
        {
            _repo = repo;
            _historyRepo = historyRepo;
        }

        /// <summary>
        /// Identify, authenticate and generate a token to a user.
        /// The token will be available for the next 30 minutes.
        /// </summary>
        /// <param name="data">Necessary data to authenticate.</param>
        /// <returns>If authenticated, return the user data.</returns>
        [HttpPost]
        [Route("Authenticate")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthenticationResult>> Authenticate(AuthenticationData data, [FromServices] ITokenGenerator tokenGenerator)
        {
            try
            {
                AuthenticationResult authenticationResult = await _repo.AuthenticateAsync(data);

                authenticationResult.Token = tokenGenerator.GenerateToken(ClaimsData.Convert(authenticationResult));

                await _repo.SaveChangesAsync();

                var historyData = new AddHistoryData()
                {
                    UserId = authenticationResult.UserId,
                    Action = HistoryAction.Authenticated
                };

                _historyRepo.AddHistoryAsync(historyData);

                return Ok(authenticationResult);
            }
            catch (Exception exception)
            {
                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }
        }

        /// <summary>
        /// Validate, create and authenticate a new account.
        /// </summary>
        /// <param name="data">Necessary data to create an account.</param>
        /// <returns>Account details.</returns>
        [HttpPost]
        [Route("New")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthenticationResult))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<AuthenticationResult>> New([FromBody] CreateAccountData data, [FromServices] ITokenGenerator tokenGenerator)
        {
            try
            {
                await _repo.CreateAsync(data);
                await _repo.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }

            var authenticationData = new AuthenticationData()
            {
                Login = data.Login,
                Password = data.Password
            };

            var result = await Authenticate(authenticationData, tokenGenerator);

            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// Change a user password.
        /// </summary>
        /// <param name="data">Necessary data to change the user's password.</param>
        [Authorize]
        [HttpPatch]
        [Route("ChangePassword")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordData data,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] IUserRepository userRepository
            )
        {
            try
            {
                var authenticatedUser = httpContextAccessor.EnsureAuthentication(userRepository);

                _repo.ChangePassword(authenticatedUser, data);
                await _repo.SaveChangesAsync();

                var historyData = new AddHistoryData()
                {
                    UserId = authenticatedUser.Id,
                    Action = HistoryAction.PasswordChanged
                };

                _historyRepo.AddHistoryAsync(historyData);

                return NoContent();
            }
            catch (Exception exception)
            {
                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }
        }

        /// <summary>
        /// Delete a user.
        /// Only users with admin roles can delete a user.
        /// </summary>
        /// <param name="id">User id.</param>
        [Authorize]
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] IUserRepository userRepository)
        {
            try
            {
                var authenticatedUser = httpContextAccessor.EnsureAuthentication(userRepository);

                await _repo.DeleteAsync(id);
                await _repo.SaveChangesAsync();

                var historyData = new AddHistoryData()
                {
                    UserId = authenticatedUser.Id,
                    Action = HistoryAction.DeletedAccount,
                    Content = new { deleted = id }
                };

                _historyRepo.AddHistoryAsync(historyData);
            }
            catch (Exception exception)
            {
                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }

            #region TODO

            //var strategy = context.Database.CreateExecutionStrategy();

            //await strategy.ExecuteAsync(async () =>
            //{
            //	await using var transaction = await context.Database.BeginTransactionAsync();

            //	try
            //	{
            //		var targetTasks = context.Entry(user).Collection(x => x.TargetTasks).Query().Select(x => x);
            //		context.Task.RemoveRange(targetTasks);

            //		var createdTasks = context.Entry(user).Collection(x => x.CreatedTasks).Query().Select(x => x);
            //		context.Task.RemoveRange(createdTasks);

            //		context.User.Remove(user);

            //		await context.SaveChangesAsync();
            //		await transaction.CommitAsync();
            //	}
            //	catch (Exception exception)
            //	{
            //		throw new ApplicationException("Could not delete user. \n" + exception.Message);
            //	}
            //});

            #endregion TODO

            return NoContent();
        }

        /// <summary>
        /// Activate an account.
        /// </summary>
        [Authorize]
        [HttpPatch]
        [Route("Activate")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Activate(
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] IUserRepository userRepository)
        {
            try
            {
                var authenticatedUser = httpContextAccessor.EnsureAuthentication(userRepository);

                await _repo.AlterStatusAsync(authenticatedUser.Id, true);
                await _repo.SaveChangesAsync();

                var historyData = new AddHistoryData()
                {
                    UserId = authenticatedUser.Id,
                    Action = HistoryAction.ActivatedAccount
                };

                _historyRepo.AddHistoryAsync(historyData);

                return NoContent();
            }
            catch (Exception exception)
            {
                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }
        }

        /// <summary>
        /// Deactivate an account.
        /// If the account has one or more pending tasks, it can't be deactivated.
        /// </summary>
        [Authorize]
        [HttpPatch]
        [Route("Deactivate")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deactivate(
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] IUserRepository userRepository)
        {
            try
            {
                var authenticatedUser = httpContextAccessor.EnsureAuthentication(userRepository);

                await _repo.AlterStatusAsync(authenticatedUser.Id, false);
                await _repo.SaveChangesAsync();

                var historyData = new AddHistoryData()
                {
                    UserId = authenticatedUser.Id,
                    Action = HistoryAction.DeactivatedAccount
                };

                _historyRepo.AddHistoryAsync(historyData);

                return NoContent();
            }
            catch (Exception exception)
            {
                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }
        }

        /// <summary>
        /// Edit users account.
        /// </summary>
        [Authorize]
        [HttpPatch]
        [Route("Edit")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Edit(
            [FromBody] EditData data,
            [FromServices] IHttpContextAccessor httpContextAccessor,
            [FromServices] IUserRepository userRepository)
        {
            try
            {
                var authenticatedUser = httpContextAccessor.EnsureAuthentication(userRepository);
                await _repo.EditAsync(authenticatedUser, data);
                await _repo.SaveChangesAsync();

                var historyData = new AddHistoryData()
                {
                    UserId = authenticatedUser.Id,
                    Action = HistoryAction.EditedAccount,
                    Content = data
                };

                _historyRepo.AddHistoryAsync(historyData);

                return NoContent();
            }
            catch (Exception exception)
            {
                int code = ExceptionController.GetStatusCode(exception);
                return StatusCode(code, exception);
            }
        }
    }
}