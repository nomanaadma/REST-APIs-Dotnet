﻿using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer;
using Movies.Contracts.Requests;
using Refit;

// var moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");

var services = new ServiceCollection();

services
	.AddHttpClient()
	.AddSingleton<AuthTokenProvider>()
	.AddRefitClient<IMoviesApi>(x => new RefitSettings
	{
		AuthorizationHeaderValueGetter = async (_, _) => await x.GetRequiredService<AuthTokenProvider>().GetTokenAsync()
	})
	.ConfigureHttpClient(x =>
		x.BaseAddress = new Uri("https://localhost:5001"));

var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();

var movie = await moviesApi.GetMovieAsync("toy-story-1995");

var newMovie = await moviesApi.CreateMovieAsync(new CreateMovieRequest
{
	Title = "Spiderman 2",
	YearOfRelease = 2002,
	Genres = ["Horror", "Comedy"]
});


await moviesApi.UpdateMovieAsync(newMovie.Id, new UpdateMovieRequest()
{
	Title = "Spiderman 2",
	YearOfRelease = 2002,
	Genres = ["Horror", "Comedy", "Adventure"]
});

await moviesApi.DeleteMovieAsync(newMovie.Id);

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



