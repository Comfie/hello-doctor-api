using System.Reflection;
using ApiBaseTemplate.Application.Common.Exceptions;
using ApiBaseTemplate.Application.Common.Interfaces;
using ApiBaseTemplate.Application.Common.Security;

namespace ApiBaseTemplate.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public AuthorizationBehaviour(
        IUser user,
        IIdentityService identityService)
    {
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        IEnumerable<AuthorizeAttribute> attributes = authorizeAttributes.ToList();
        if (attributes.Any())
        {
            // Must be authenticated user
            if (_user.Id == null)
            {
                throw new UnauthorizedAccessException();
            }

            // Role-based authorization
            var authorizeAttributesWithRoles = attributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            IEnumerable<AuthorizeAttribute> attributesWithRoles = authorizeAttributesWithRoles.ToList();
            if (attributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in attributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        var isInRole = await _identityService.IsInRoleAsync(_user.Id, role.Trim(), cancellationToken);
                        if (isInRole.IsSuccess)
                        {
                            authorized = true;
                            break;
                        }
                    }
                }

                // Must be a member of at least one role in roles
                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }
            }

            // Policy-based authorization
            var authorizeAttributesWithPolicies = attributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
            IEnumerable<AuthorizeAttribute> attributesWithPolicies = authorizeAttributesWithPolicies.ToList();
            if (attributesWithPolicies.Any())
            {
                foreach (var policy in attributesWithPolicies.Select(a => a.Policy))
                {
                    var authorized = await _identityService.AuthorizeAsync(_user.Id, policy, cancellationToken);

                    if (!authorized.IsSuccess)
                    {
                        throw new ForbiddenAccessException();
                    }
                }
            }
        }

        // User is authorized / authorization not required
        return await next();
    }
}
