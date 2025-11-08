using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using HelloDoctorApi.Application.AdminDashboard.Models;
using HelloDoctorApi.Application.AdminDashboard.Queries.GetAdminDashboardStats;
using HelloDoctorApi.Application.Authentications.Commands.CreateRole;
using HelloDoctorApi.Application.Authentications.Commands.GetAllUsers;
using HelloDoctorApi.Application.Authentications.Models;
using HelloDoctorApi.Application.Authentications.Queries.GetRoles;
using HelloDoctorApi.Application.Authentications.Queries.GetUserRoles;
using HelloDoctorApi.Application.Authentications.Updates.ChangeRoleStatus;
using HelloDoctorApi.Application.Authentications.Updates.RevokeUserRole;
using HelloDoctorApi.Application.Authentications.Updates.UpdateUserRole;
using HelloDoctorApi.Application.Common.Security;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoctorApi.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[TranslateResultToActionResult]
[Authorize(Roles = "SystemAdministrator,SuperAdministrator")]
public class SystemAdminController : ApiController
{
    public SystemAdminController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    ///     Get admin dashboard statistics
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("dashboard-stats")]
    [ProducesResponseType(typeof(AdminDashboardStatsResponse), StatusCodes.Status200OK)]
    public async Task<Result<AdminDashboardStatsResponse>> GetDashboardStats(CancellationToken cancellationToken = default)
    {
        var response = await Sender.Send(new GetAdminDashboardStatsCommand(), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Get all users
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("get-all-users")]
    public async Task<Result<List<UserDetailsResponse>>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        var
            response = await Sender.Send(new GetAllUsersCommand(), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Create role
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("create-role")]
    public async Task<Result<bool>> CreateRole([FromBody] CreateRoleCommand command)
    {
        var response = await Sender.Send(command);
        return response;
    }

    /// <summary>
    ///     Get all roles
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("get-roles")]
    public async Task<Result<List<UserRoleResponse>>> GetRoles(CancellationToken cancellationToken = default)
    {
        var response = await Sender.Send(new GetRolesCommand(), cancellationToken);
        return response;
    }
    
    /// <summary>
    ///     Get user roles
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("get-user-roles/{id}")]
    public async Task<Result<List<string>>> GetUserRoles( string id, CancellationToken cancellationToken = default)
    {
        var response = await Sender.Send(new GetUserRolesCommand(id), cancellationToken);
        return response;
    }
    

    /// <summary>
    ///     Update user role
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("update-user-role")]
    public async Task<Result<bool>> UpdateUserRole([FromBody] UpdateUserRoleCommand command)
    {
        var response = await Sender.Send(command);
        return response;
    }

    /// <summary>
    ///     Change role status
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("update-role-status")]
    public async Task<Result<bool>> ChangeRoleStatus([FromBody] ChangeRoleStatusCommand command)
    {
        var response = await Sender.Send(command);
        return response;
    }

    /// <summary>
    ///     Revoke role for the user
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("revoke-role")]
    public async Task<Result<bool>> RevokeRole([FromBody] RevokeUserRoleCommand command)
    {
        var response = await Sender.Send(command);
        return response;
    }
}