using Microsoft.AspNetCore.Mvc;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Api.Mapping;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController(IMovieRepository movieRepository) : ControllerBase
{
    private readonly IMovieRepository _movieRepository = movieRepository;

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody]CreateMovieRequest request)
    {
        var movie = request.MapToMovie();
         
        await _movieRepository.CreateAsync(movie);
        return Created($"/{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
    }
    
}