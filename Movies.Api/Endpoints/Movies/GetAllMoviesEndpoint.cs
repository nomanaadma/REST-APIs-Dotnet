using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Movies;

public static class GetAllMoviesEndpoint
{
	public const string Name = "GetAllMovies";

	public static IEndpointRouteBuilder MapGetAllMovie(this IEndpointRouteBuilder app)
	{
		app.MapGet(ApiEndpoints.Movies.GetAll, async (
			[AsParameters]GetAllMoviesRequest request,
			IMovieService movieService,
			HttpContext context,
			CancellationToken token
			) =>
		{
			var userId = context.GetUserId();
			var options = request.MapToOptions()
				.WithUserId(userId);
            
			var movies = await movieService.GetAllAsync(options, token);
			var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, token);
			var response = movies.MapToResponse(options.Page, options.PageSize, movieCount);
			return TypedResults.Ok(response);
			
		})
		.WithName($"{Name}V1")
		.Produces<MoviesResponse>(StatusCodes.Status200OK)
		.WithApiVersionSet(ApiVersioning.VersionSet)
		.HasApiVersion(1.0);

		app.MapGet(ApiEndpoints.Movies.GetAll, async (
				[AsParameters] GetAllMoviesRequest request,
				IMovieService movieService,
				HttpContext context,
				CancellationToken token
			) =>
			{
				var userId = context.GetUserId();
				var options = request.MapToOptions()
					.WithUserId(userId);

				var movies = await movieService.GetAllAsync(options, token);
				var movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, token);
				var response = movies.MapToResponse(options.Page, options.PageSize, movieCount);
				return TypedResults.Ok(response);

			})
			.WithName($"{Name}V2")
			.Produces<MoviesResponse>(StatusCodes.Status200OK)
			.WithApiVersionSet(ApiVersioning.VersionSet)
			.HasApiVersion(2.0)
			.CacheOutput("MovieCache");
		
		return app;
	}
}