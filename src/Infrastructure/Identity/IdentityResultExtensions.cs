using ApiBaseTemplate.Application.Common.Models;
using ApiBaseTemplate.Domain.Shared;
using Microsoft.AspNetCore.Identity;

namespace ApiBaseTemplate.Infrastructure.Identity
{
    public static class IdentityResultExtensions
    {
        public static Result<string> ToApplicationResult(this IdentityResult result)
        {
            return result.Succeeded
                ? Result.Success<string>("Operation completed successfully.")
                : Result.Failure<string>(new Error("IdentityResult", result.Errors.Select(e => e.Description).ToString() ?? "Failed to complete operation"));
        }

    }
}
