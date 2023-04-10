using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class MessageValidator : AbstractValidator<Message>
{
    public MessageValidator()
    {
        RuleFor(m => m.Content).NotEmpty().MaximumLength(500);
    }
}