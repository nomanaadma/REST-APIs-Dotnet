﻿using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Ratings;

public static class DeleteRatingEndpoint
{
	public const string Name = "DeleteRating";
	
	public static IEndpointRouteBuilder MapDeleteRating(this IEndpointRouteBuilder app)
	{
		app.MapDelete(ApiEndpoints.Movies.DeleteRating, async (
			Guid id,
			HttpContext context, 
			IRatingService ratingService,
			CancellationToken token
		) =>
		{
			var userid = context.GetUserId();
			var result = await ratingService.DeleteRatingAsync(id, userid!.Value, token);
			return result ? Results.Ok() : Results.NotFound();
		}).WithName(Name);

		return app;
	}

}