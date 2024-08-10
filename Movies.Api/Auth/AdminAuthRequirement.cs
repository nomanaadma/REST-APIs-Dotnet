using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Movies.Api.Auth;

public class AdminAuthRequirement : IAuthorizationHandler, IAuthorizationRequirement
{

	private readonly string _apiKey;

	// ReSharper disable once ConvertToPrimaryConstructor
	public AdminAuthRequirement(string apiKey)
	{
		_apiKey = apiKey;
	}

	public Task HandleAsync(AuthorizationHandlerContext context)
	{
		if (context.User.HasClaim(AuthConstants.AdminClaim, "true"))
		{
			context.Succeed(this);
			return Task.CompletedTask;
		}

		if (context.Resource is not HttpContext httpContext)
		{
			return Task.CompletedTask;
		}
		
		if (!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName,
			    out var extractedApiKey))
		{
			context.Fail();
			return Task.CompletedTask;
		}

		if (_apiKey != extractedApiKey)
		{
			context.Fail();
			return Task.CompletedTask;
		}

		var identity = (ClaimsIdentity)httpContext.User.Identity!;
		
		identity.AddClaim(new Claim("userid", Guid.Parse("d8566de3-b1a6-4a9b-b842-8e3887a82e42").ToString()));

		context.Succeed(this);

		return Task.CompletedTask;

	}
}

