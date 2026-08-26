using FluentValidation;
using KatameApi.DTOs.Finance;

namespace KatameApi.Validators;

public class UpdateFinancialProfileValidator : AbstractValidator<UpdateFinancialProfileDto>
{
    public UpdateFinancialProfileValidator()
    {
        RuleFor(x => x.MonthlyIncome)
            .GreaterThanOrEqualTo(0).WithMessage("El ingreso mensual no puede ser negativo.");
    }
}
