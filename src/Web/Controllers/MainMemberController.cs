using Ardalis.Result.AspNetCore;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace HelloDoctorApi.Web.Controllers;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[TranslateResultToActionResult]
public class MainMemberController : ApiController
{
    public MainMemberController(ISender sender) : base(sender)
    {
    }
}