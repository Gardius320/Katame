using FluentValidation.TestHelper;
using KatameApi.DTOs.Finance;
using KatameApi.Models;
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
    public void CreateSavingsGoalValidator_falla_si_el_ahorro_mensual_planeado_es_cero()
    {
        var result = new CreateSavingsGoalValidator().TestValidate(
            new CreateSavingsGoalDto
            {
                Name = "Carro",
                TargetAmount = 28_000_000,
                CurrentAmount = 8_000_000,
                MonthlyContributionTarget = 0,
            });
        result.ShouldHaveValidationErrorFor(x => x.MonthlyContributionTarget);
    }

    [Fact]
    public void CreateSavingsGoalValidator_pasa_si_no_se_define_ahorro_mensual_planeado()
    {
        var result = new CreateSavingsGoalValidator().TestValidate(
            new CreateSavingsGoalDto
            {
                Name = "Carro",
                TargetAmount = 28_000_000,
                CurrentAmount = 8_000_000,
                MonthlyContributionTarget = null,
            });
        result.ShouldNotHaveValidationErrorFor(x => x.MonthlyContributionTarget);
    }

    [Fact]
    public void UpdateFinancialProfileValidator_falla_si_el_ingreso_es_negativo()
    {
        var result = new UpdateFinancialProfileValidator().TestValidate(
            new UpdateFinancialProfileDto { MonthlyIncome = -1 });
        result.ShouldHaveValidationErrorFor(x => x.MonthlyIncome);
    }

    [Fact]
    public void UpdateFinancialProfileValidator_pasa_si_el_ingreso_es_cero_o_positivo()
    {
        var result = new UpdateFinancialProfileValidator().TestValidate(
            new UpdateFinancialProfileDto { MonthlyIncome = 5_000_000 });
        result.ShouldNotHaveValidationErrorFor(x => x.MonthlyIncome);
    }

    [Fact]
    public void CreateObligationValidator_falla_si_el_nombre_esta_vacio()
    {
        var result = new CreateObligationValidator().TestValidate(
            new CreateObligationDto { Name = "", Amount = 100, DueDate = DateTime.UtcNow });
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreateObligationValidator_falla_si_es_recurrente_sin_frecuencia()
    {
        var result = new CreateObligationValidator().TestValidate(
            new CreateObligationDto
            {
                Name = "Alquiler",
                Amount = 100,
                DueDate = DateTime.UtcNow,
                IsRecurring = true,
                RecurrenceFrequency = null,
            });
        result.ShouldHaveValidationErrorFor(x => x.RecurrenceFrequency);
    }

    [Fact]
    public void CreateObligationValidator_pasa_si_es_recurrente_con_frecuencia()
    {
        var result = new CreateObligationValidator().TestValidate(
            new CreateObligationDto
            {
                Name = "Alquiler",
                Amount = 100,
                DueDate = DateTime.UtcNow,
                IsRecurring = true,
                RecurrenceFrequency = RecurrenceFrequency.Monthly,
            });
        result.ShouldNotHaveValidationErrorFor(x => x.RecurrenceFrequency);
    }

    [Fact]
    public void CreateObligationValidator_pasa_si_no_es_recurrente_sin_frecuencia()
    {
        var result = new CreateObligationValidator().TestValidate(
            new CreateObligationDto
            {
                Name = "Internet",
                Amount = 100,
                DueDate = DateTime.UtcNow,
                IsRecurring = false,
                RecurrenceFrequency = null,
            });
        result.ShouldNotHaveValidationErrorFor(x => x.RecurrenceFrequency);
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
