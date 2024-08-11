using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Contracts.Requests;
using Refit;

// var moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");

var serivces = new ServiceCollection();

serivces.AddRefitClient<IMoviesApi>()
	.ConfigureHttpClient(x =>
		x.BaseAddress = new Uri("https://localhost:5001"));

var provider = serivces.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();


// var movie = await moviesApi.GetMovieAsync("toy-story-1995");

var request = new GetAllMoviesRequest
{
	Title = null,
	Year = null,
	SortBy = null,
	Page = 1,
	PageSize = 3,
};

var movies = await moviesApi.GetMoviesAsync(request);

foreach (var movieResponse in movies.Items)
{
	Console.WriteLine(JsonSerializer.Serialize(movieResponse));
}

// Console.WriteLine(JsonSerializer.Serialize(movie));



