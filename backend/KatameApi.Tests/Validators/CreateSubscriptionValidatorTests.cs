using FluentValidation.TestHelper;
using KatameApi.DTOs.Subscriptions;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class CreateSubscriptionValidatorTests
{
    [Fact]
    public void CreateSubscriptionValidator_falla_si_el_nombre_esta_vacio()
    {
        var result = new CreateSubscriptionValidator().TestValidate(
            new CreateSubscriptionDto { Name = "", Amount = 15, RenewalDate = DateTime.UtcNow });
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreateSubscriptionValidator_falla_si_el_monto_es_cero()
    {
        var result = new CreateSubscriptionValidator().TestValidate(
            new CreateSubscriptionDto { Name = "Netflix", Amount = 0, RenewalDate = DateTime.UtcNow });
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }
}
