using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Id.ToString()).NotEmpty();
        RuleFor(x => x.DisplayName).NotEmpty().Length(1, 16);
        RuleFor(x => x.FullName).NotEmpty().Length(1, 128);
        RuleFor(x => x.Status).NotEmpty().Length(1, 100);
    }
}