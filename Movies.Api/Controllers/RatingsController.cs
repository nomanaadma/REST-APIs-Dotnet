using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;

    
    // ReSharper disable once ConvertToPrimaryConstructor
    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }
    
    [Authorize]
    [HttpPut(ApiEndpoints.Movies.Rate)]
    public async Task<IActionResult> RateMovie([FromRoute] Guid id, 
        [FromBody] RateMovieRequest request, CancellationToken token)
    {
        var userId = HttpContext.GetUserId();
        var result = await _ratingService.RateMovieAsync(id, request.Rating, userId!.Value, token);

        return result ? Ok() : NotFound();

    }

    [Authorize]
    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id,
        CancellationToken token)
    {
        var userid = HttpContext.GetUserId();
        var result = await _ratingService.DeleteRatingAsync(id, userid!.Value, token);
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
    public async Task<IActionResult> GetUserRatings(CancellationToken token)
    {
        var userid = HttpContext.GetUserId();
        var ratings = await _ratingService.GetRatingsForUserAsync(userid!.Value, token);
        var ratingsReponse = ratings.MapToResponse();
        return Ok(ratingsReponse);
    }

}