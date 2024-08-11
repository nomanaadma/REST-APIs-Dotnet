using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Movies;

public static class DeleteMovieEndpoint
{
	public const string Name = "DeleteMovie";

	public static IEndpointRouteBuilder MapDeleteMovie(this IEndpointRouteBuilder app)
	{
		app.MapDelete(ApiEndpoints.Movies.Delete, async (
			Guid id,
			IMovieService movieService,
			IOutputCacheStore outputCacheStore,
			HttpContext context,
			CancellationToken token
			) =>
		{
			var userid = context.GetUserId();
			var deleted = await movieService.DeleteByIdAsync(id, token);
			if (!deleted)
				return Results.NotFound();
        
			await outputCacheStore.EvictByTagAsync("movies", token);

			return TypedResults.Ok();
		}).WithName(Name);

		return app;
	}
	
}