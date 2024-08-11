using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Endpoints.Ratings;

public static class GetUserRatingsEndpoint
{
	public const string Name = "GetUserRatings";
	
	public static IEndpointRouteBuilder MapGetUserRatings(this IEndpointRouteBuilder app)
	{
		app.MapPut(ApiEndpoints.Ratings.GetUserRatings, async (
			HttpContext context, IRatingService ratingService, CancellationToken token
		) =>
		{
			var userid = context.GetUserId();
			var ratings = await ratingService.GetRatingsForUserAsync(userid!.Value, token);
			var ratingsResponse = ratings.MapToResponse();
			return Results.Ok(ratingsResponse);
			
		}).WithName(Name);
		
		return app;
	}

}