using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using HelloDoctorApi.Application.Authentications.Commands.AuthenticateUser;
using HelloDoctorApi.Application.Authentications.Commands.CreateUserCommand;
using HelloDoctorApi.Application.Authentications.Commands.GetUserById;
using HelloDoctorApi.Application.Authentications.Commands.RefreshToken;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Authentications.Updates.UpdateUser;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoctorApi.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[TranslateResultToActionResult]
public class AuthenticationController : ApiController
{
    public AuthenticationController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    ///     Login
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Result<AuthResponse>> Login([FromBody] AuthenticateUserCommand command)
    {
        var response = await Sender.Send(command);
        return response;
    }

    /// <summary>
    ///     Refresh token
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Result<AuthResponse>> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var response = await Sender.Send(command);
        return response;
    }


    /// <summary>
    ///     Create user
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<Result<bool>> Create([FromBody] CreateUserCommand command)
    {
        var response = await Sender.Send(command);
        return response;
    }

    

    /// <summary>
    ///     Get user by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("get-user-by-id/{id}")]
    public async Task<Result<UserDetailsResponse>> GetUserById(string id, CancellationToken cancellationToken = default)
    {
        var
            response = await Sender.Send(new GetUserByIdCommand(id), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Update user
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("update-user")]
    public async Task<Result<UserDetailsResponse>> UpdateUser([FromBody] UpdateUserCommand command)
    {
        var response = await Sender.Send(command);
        return response;
    }
    
}