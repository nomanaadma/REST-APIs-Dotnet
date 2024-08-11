using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Contracts.Requests;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
// [ApiVersion(2.0)]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IOutputCacheStore _outputCacheStore;

    // ReSharper disable once ConvertToPrimaryConstructor
    public MoviesController(IMovieService movieService, IOutputCacheStore outputCacheStore)
    {
        _movieService = movieService;
        _outputCacheStore = outputCacheStore;
    }

    // [Authorize(AuthConstants.AdminPolicy)]
    // [ServiceFilter(typeof(ApiKeyAuthFilter))]
    [HttpPost(ApiEndpoints.Movies.Create)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody]CreateMovieRequest request, CancellationToken token)
    {
        var movie = request.MapToMovie();
        await _movieService.CreateAsync(movie, token);
        await _outputCacheStore.EvictByTagAsync("movies", token);
        var response = movie.MapToResponse();
        return CreatedAtAction(nameof(GetV1), new { idOrSlug = movie.Id }, response);
    }
    
    // [Authorize]
    [MapToApiVersion(1.0)]
    [HttpGet(ApiEndpoints.Movies.Get)]
    // [ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
    [OutputCache]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetV1([FromRoute] string idOrSlug,
        CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, userId, token)
            : await _movieService.GetBySlugAsync(idOrSlug, userId, token);
            
        if (movie is null)
            return NotFound();

        var response = movie.MapToResponse();
        return Ok(response);
    }

    // [Authorize]
    // [AllowAnonymous]
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    // [ResponseCache(Duration = 30, VaryByQueryKeys = ["title", "year", "sortBy", "page", "pageSize"] ,VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
    [OutputCache(PolicyName = "MovieCache")]
    [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllMoviesRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var options = request.MapToOptions()
            .WithUserId(userId);
            
        var movies = await _movieService.GetAllAsync(options, token);
        var movieCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, token);
        var response = movies.MapToResponse(options.Page, options.PageSize, movieCount);
        return Ok(response);
    }
    
    // [AllowAnonymous]
    [HttpPut(ApiEndpoints.Movies.Update)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, 
        [FromBody] UpdateMovieRequest request, 
        CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, userId, token);
        
        if (updatedMovie is null)
            return NotFound();
        
        await _outputCacheStore.EvictByTagAsync("movies", token);

        var response = updatedMovie.MapToResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminPolicy)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var userid = HttpContext.GetUserId();
        var deleted = await _movieService.DeleteByIdAsync(id, token);
        if (!deleted)
            return NotFound();
        
        await _outputCacheStore.EvictByTagAsync("movies", token);

        return Ok();
    }
    
    // [MapToApiVersion(2.0)]
    // [HttpGet(ApiEndpoints.Movies.Get)]
    // public async Task<IActionResult> GetV2([FromRoute] string idOrSlug,
    //     CancellationToken token)
    // {
    //     var userId = HttpContext.GetUserId();
    //     
    //     var movie = Guid.TryParse(idOrSlug, out var id)
    //         ? await _movieService.GetByIdAsync(id, userId, token)
    //         : await _movieService.GetBySlugAsync(idOrSlug, userId, token);
    //         
    //     if (movie is null)
    //         return NotFound();
    //
    //     var response = movie.MapToResponse();
    //     return Ok(response);
    // }
    
    // temp function to bulk insert json data in database
    // [Authorize]
    // [HttpPost(ApiEndpoints.Movies.CreateBulk)]
    // public async Task<IActionResult> CreateBulk([FromBody] IEnumerable<CreateMovieRequest> request, CancellationToken token)
    // {
    //     var movies = request.MapToResponse();
    //
    //     foreach (var movie in movies)
    //     {
    //         await _movieService.CreateAsync(movie, token);    
    //     }
    //     
    //     Console.WriteLine("Debug");
    //     return Ok();
    //     
    // }
    
}