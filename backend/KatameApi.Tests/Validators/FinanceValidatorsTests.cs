using FluentValidation.TestHelper;
using KatameApi.DTOs.Finance;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class FinanceValidatorsTests
{
    [Fact]
    public void CreateSavingsGoalValidator_falla_si_la_meta_es_cero()
    {
        var result = new CreateSavingsGoalValidator().TestValidate(
            new CreateSavingsGoalDto { Name = "Vacaciones", TargetAmount = 0, CurrentAmount = 0 });
        result.ShouldHaveValidationErrorFor(x => x.TargetAmount);
    }

    [Fact]
    public void CreateObligationValidator_falla_si_el_nombre_esta_vacio()
    {
        var result = new CreateObligationValidator().TestValidate(
            new CreateObligationDto { Name = "", Amount = 100, DueDate = DateTime.UtcNow });
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void CreateCreditCardValidator_falla_si_el_dia_de_corte_es_invalido(int statementDay)
    {
        var result = new CreateCreditCardValidator().TestValidate(
            new CreateCreditCardDto { Name = "Visa", StatementDay = statementDay, PaymentDay = 5, CreditLimit = 1000 });
        result.ShouldHaveValidationErrorFor(x => x.StatementDay);
    }
}
