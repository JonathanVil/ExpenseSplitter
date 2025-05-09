using FluentValidation;

namespace ExpenseSplitter.Shared.Utils;

public static class CustomValidators
{
    public static IRuleBuilderOptions<T, string> IsValidGuid<T>(this IRuleBuilder<T, string> ruleBuilder) {

        return ruleBuilder
            .Must(value => Guid.TryParse(value, out _))
            .WithMessage("{PropertyName} must be a valid GUID");
    }
}