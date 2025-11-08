using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using HelloDoctorApi.Application.Notifications.Commands.UpdatePreferences;
using HelloDoctorApi.Application.Notifications.Models;
using HelloDoctorApi.Application.Notifications.Queries.GetMyNotifications;
using HelloDoctorApi.Application.Notifications.Queries.GetMyPreferences;
using HelloDoctorApi.Application.Notifications.Updates.MarkAsRead;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoctorApi.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[TranslateResultToActionResult]
public class NotificationController : ApiController
{
    public NotificationController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    ///     Get my notifications
    /// </summary>
    /// <param name="unreadOnly">Filter for unread notifications only</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of notifications</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<Result<List<NotificationResponse>>> GetMyNotifications(
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var response = await Sender.Send(new GetMyNotificationsQuery(unreadOnly, pageNumber, pageSize), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Mark a notification as read
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Success result</returns>
    [HttpPut("{id}/mark-as-read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<Result> MarkAsRead(long id, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new MarkNotificationAsReadCommand(id), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Get my notification preferences
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>List of notification preferences</returns>
    [HttpGet("preferences")]
    [ProducesResponseType(typeof(List<NotificationPreferenceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<Result<List<NotificationPreferenceResponse>>> GetMyPreferences(CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetMyPreferencesQuery(), cancellationToken);
        return response;
    }

    /// <summary>
    ///     Update my notification preferences
    /// </summary>
    /// <param name="command">Notification preferences to update</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Success result</returns>
    [HttpPut("preferences")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<Result> UpdatePreferences([FromBody] UpdateNotificationPreferencesCommand command, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(command, cancellationToken);
        return response;
    }
}
