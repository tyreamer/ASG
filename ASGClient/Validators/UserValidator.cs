using ASGShared.Models;
using FluentValidation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(1, 100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .Length(1, 100);

        RuleFor(x => x.HouseholdSize)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.CookingSkillLevel)
            .InclusiveBetween(1, 10);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<User>.CreateWithOptions((User)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
        {
            return Array.Empty<string>();
        }
        return result.Errors.Select(e => e.ErrorMessage);
    };
}
