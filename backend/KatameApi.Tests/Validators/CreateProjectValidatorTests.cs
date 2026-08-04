using FluentValidation.TestHelper;
using KatameApi.DTOs.Projects;
using KatameApi.Validators;

namespace KatameApi.Tests.Validators;

public class CreateProjectValidatorTests
{
    [Fact]
    public void CreateProjectValidator_falla_si_el_nombre_esta_vacio()
    {
        var result = new CreateProjectValidator().TestValidate(
            new CreateProjectDto { Name = "", Description = "", Status = "active" });
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CreateProjectValidator_falla_si_el_estado_no_es_valido()
    {
        var result = new CreateProjectValidator().TestValidate(
            new CreateProjectDto { Name = "Proyecto", Description = "", Status = "invalido" });
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }
}
