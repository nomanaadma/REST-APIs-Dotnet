using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators;

public class GetAllMoviesOptionsValidators : AbstractValidator<GetAllMoviesOptions>
{
    private static readonly string[] AcceptableSortFields =
    [
        "title", "year"
    ];
    public GetAllMoviesOptionsValidators()
    {
        RuleFor(m => m.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(m => m.SortField)
            .Must(x => x is null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage("You can only sort by title and year");

        RuleFor(m => m.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(m => m.PageSize)
            .InclusiveBetween(1, 25)
            .WithMessage("You can get between 1 and 25 movies per page.");

    }
}