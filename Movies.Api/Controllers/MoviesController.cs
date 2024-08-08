using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Contracts.Requests;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    // ReSharper disable once ConvertToPrimaryConstructor
    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [Authorize(AuthConstants.AdminPolicy)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody]CreateMovieRequest request, CancellationToken token)
    {
        var movie = request.MapToMovie();
        await _movieService.CreateAsync(movie, token);
        var response = movie.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, response);
    }
    
    // [Authorize]
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, 
        [FromServices] LinkGenerator linkGenerator,
        CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, userId, token)
            : await _movieService.GetBySlugAsync(idOrSlug, userId, token);
            
        if (movie is null)
            return NotFound();

        var response = movie.MapToResponse();

        response.Links.Add(new Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Get), values: new {idOrSlug = movie.Id }),
            Rel = "self",
            Type = "GET"
        });
        
        response.Links.Add(new Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Update), values: new {idOrSlug = movie.Id }),
            Rel = "self",
            Type = "UPDATE"
        });
        
        response.Links.Add(new Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Delete), values: new {idOrSlug = movie.Id }),
            Rel = "self",
            Type = "DELETE"
        });

        
        return Ok(response);
    }

    // [Authorize]
    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Movies.GetAll)]
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
    
    [AllowAnonymous]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, 
        [FromBody] UpdateMovieRequest request, 
        CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, userId, token);
        
        if (updatedMovie is null)
            return NotFound();

        var response = updatedMovie.MapToResponse();
        return Ok(response);

    }

    [Authorize(AuthConstants.TrustedMember)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var deleted = await _movieService.DeleteByIdAsync(id, token);
        if (!deleted)
            return NotFound();

        return Ok();
    }   
    
    
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