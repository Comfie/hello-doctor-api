using ApiBaseTemplate.Application.Authentications.Commands.AuthenticateUser;
using ApiBaseTemplate.Application.Authentications.Commands.CreateUserCommand;
using ApiBaseTemplate.Application.Authentications.Commands.GetAllUsers;
using ApiBaseTemplate.Application.Authentications.Commands.GetUserById;
using ApiBaseTemplate.Application.Authentications.Commands.UpdateUser;
using ApiBaseTemplate.Application.Authentications.Models;
using ApiBaseTemplate.Domain.Shared;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiBaseTemplate.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthenticationController : ApiController
{
    public AuthenticationController(ISender sender)
        : base(sender)
    {
    }

    /// <summary>
    /// Get user by id
    /// </summary>
    /// <param name="id">user identifier</param>
    /// <param name="cancellationToken"></param>
    /// <returns>user details</returns>
    [Authorize]
    [HttpGet("get-user/{id}")]
    public async Task<IActionResult> GetUserById(string id, CancellationToken cancellationToken)
    {
        Result<UserDetailsResponse> response = await Sender.Send(new GetUserByIdCommand(id), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="request">the deatils for the user</param>
    /// <param name="cancellationToken"></param>
    /// <returns>custom response with message</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(UserDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand()
        {
            Email = request.Email,
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            PhoneNumber = request.PhoneNumber,
            Password = request.Password
        };

        Result<bool> result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return CreatedAtAction(
            nameof(GetUserById),
            new { id = result.Value },
            result.Value);
    }

    /// <summary>
    /// Authenticate user
    /// </summary>
    /// <param name="request">the deatils for the user</param>
    /// <param name="cancellationToken"></param>
    /// <returns>custom response with message</returns>
    [HttpPost("authenticate")]
    public async Task<IActionResult> AuthenticateUser(
        [FromBody] UserAuthRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AuthenticateUserCommand() { Email = request.Email, Password = request.Password };

        Result<AuthResponse> response = await Sender.Send(command, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    /// <summary>
    /// Get all users
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>list of users</returns>
    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        Result<List<UserDetailsResponse>> response = await Sender.Send(new GetAllUsersCommand(), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateUserRequest"></param>
    /// <returns>user details</returns>
    [HttpPut("update-user/{id}")]
    [ProducesResponseType(typeof(UserDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDetailsResponse>> UpdateUserDetails(string id,
        [FromBody] UpdateUserRequest updateUserRequest)
    {
        Result<UserDetailsResponse> response =
            await Sender.Send(new UpdateUserCommand(id, updateUserRequest), CancellationToken.None);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }
}
