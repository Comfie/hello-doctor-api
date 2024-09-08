using Asp.Versioning;
using HelloDoctorApi.Application.Authentications.Commands.AuthenticateUser;
using HelloDoctorApi.Application.Authentications.Commands.CreateUserCommand;
using HelloDoctorApi.Application.Authentications.Commands.GetAllUsers;
using HelloDoctorApi.Application.Authentications.Commands.GetUserById;
using HelloDoctorApi.Application.Authentications.Commands.UpdateUser;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Authentications.Queries.GetUserRoles;
using HelloDoctorApi.Application.Authentications.Updates.UpdateUserRole;
using HelloDoctorApi.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoctorApi.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthenticationController : ApiController
{
    public AuthenticationController(ISender sender) : base(sender)
    {
    }
    
    /// <summary>
    /// Login
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] AuthenticateUserCommand command)
    {
        Result<AuthResponse> response = await Sender.Send(command);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }
    
    /// <summary>
    /// Create user
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        Result<bool> response = await Sender.Send(command);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }
    
    /// <summary>
    /// Get all users
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        Result<List<UserDetailsResponse>>
            response = await Sender.Send(new GetAllUsersCommand(), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }
    
    /// <summary>
    /// Get user by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("get-user-by-id/{id}")]
    public async Task<IActionResult> GetUserById(string id, CancellationToken cancellationToken = default)
    {
        Result<UserDetailsResponse>
            response = await Sender.Send(new GetUserByIdCommand(id), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }
    
    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("update-user")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command)
    {
        Result<UserDetailsResponse> response = await Sender.Send(command);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }
    
    /// <summary>
    /// Get user roles
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("get-user-roles")]
    public async Task<IActionResult> GetUserRoles(CancellationToken cancellationToken = default)
    {
        Result<List<string?>> response = await Sender.Send(new GetUserRolesCommand(), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }
    
    /// <summary>
    /// Update user role
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("update-user-role")]
    public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleCommand command)
    {
        Result<bool> response = await Sender.Send(command);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }
}