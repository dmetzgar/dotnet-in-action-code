using Microsoft.AspNetCore.Authorization;

namespace ManningBooksApi;

public class OnlyMeRequirement : 
  IAuthorizationRequirement { }

public class OnlyMeRequirementHandler :
  AuthorizationHandler<OnlyMeRequirement>
{
  protected override Task HandleRequirementAsync(
    AuthorizationHandlerContext context, 
    OnlyMeRequirement requirement)
  {
    if (context.User != null)
    {
      var emailClaim = context.User
        .FindFirst("preferred_username");
      if (emailClaim != null 
        && emailClaim.Value == "your@email.com")
      {
        context.Succeed(requirement);
      }
    }

    return Task.CompletedTask;
  }
}
