using AutoMapper;
using KatameApi.DTOs.Finance;
using KatameApi.Models;
using KatameApi.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace KatameApi.Tests.Services;

public class CreditCardServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<FinanceMappingProfile>(), NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    private static CreditCardService CreateService(
        FakeCreditCardRepository creditCards, FakeTransactionRepository transactions) =>
        new(creditCards, transactions, CreateMapper());

    [Fact]
    public async Task GetAllAsync_calcula_el_gastado_del_ciclo_desde_el_ultimo_corte()
    {
        var today = DateTime.UtcNow.Date;
        // Corte hace 10 días. Se usa un offset de 10 (en vez de 1) para que el
        // cálculo sea estable sin importar qué día del mes corre el test: con
        // offsets chicos, el "día de corte" resultante podría coincidir con un
        // día que también es válido dentro del mes actual y hacer que
        // BillingCycle elija el mes equivocado.
        var statementDay = today.AddDays(-10).Day;
        var lastStatementDate = today.AddDays(-10);

        var creditCards = new FakeCreditCardRepository();
        await creditCards.AddAsync(new CreditCard
        {
            Name = "Visa",
            StatementDay = statementDay,
            PaymentDay = statementDay,
            CreditLimit = 1000,
        });
        var card = (await creditCards.GetAllAsync())[0];

        var transactions = new FakeTransactionRepository();
        // Dentro del ciclo abierto (después del corte, hasta hoy) -> cuenta.
        await transactions.AddAsync(new Transaction { Amount = 100, Type = "expense", Category = "Comida", Date = today.AddDays(-5), CreditCardId = card.Id });
        await transactions.AddAsync(new Transaction { Amount = 50, Type = "expense", Category = "Transporte", Date = today, CreditCardId = card.Id });
        // Antes del corte -> no cuenta.
        await transactions.AddAsync(new Transaction { Amount = 999, Type = "expense", Category = "Ciclo anterior", Date = today.AddDays(-11), CreditCardId = card.Id });
        // Ingreso (no es gasto) -> no cuenta.
        await transactions.AddAsync(new Transaction { Amount = 300, Type = "income", Category = "Reembolso", Date = today.AddDays(-3), CreditCardId = card.Id });
        // Gasto de otra tarjeta -> no cuenta.
        await transactions.AddAsync(new Transaction { Amount = 555, Type = "expense", Category = "Otra tarjeta", Date = today.AddDays(-3), CreditCardId = 999 });

        var service = CreateService(creditCards, transactions);

        var result = await service.GetAllAsync();

        var dto = Assert.Single(result);
        Assert.Equal(150, dto.CycleUsage);
    }

    [Fact]
    public async Task GetAllAsync_devuelve_cero_si_no_hay_gastos_en_el_ciclo_abierto()
    {
        var creditCards = new FakeCreditCardRepository();
        await creditCards.AddAsync(new CreditCard { Name = "Sin uso", StatementDay = 5, PaymentDay = 20, CreditLimit = 500 });

        var service = CreateService(creditCards, new FakeTransactionRepository());

        var result = await service.GetAllAsync();

        var dto = Assert.Single(result);
        Assert.Equal(0, dto.CycleUsage);
    }

    [Fact]
    public async Task CreateAsync_guarda_el_logo_del_banco()
    {
        var service = CreateService(new FakeCreditCardRepository(), new FakeTransactionRepository());

        var created = await service.CreateAsync(new CreateCreditCardDto
        {
            Name = "Visa Gold",
            StatementDay = 5,
            PaymentDay = 20,
            CreditLimit = 1000,
            LogoDataUrl = "data:image/png;base64,ABC123",
        });

        Assert.Equal("data:image/png;base64,ABC123", created.LogoDataUrl);
    }

    [Fact]
    public async Task UpdateAsync_permite_reemplazar_o_quitar_el_logo()
    {
        var creditCards = new FakeCreditCardRepository();
        var service = CreateService(creditCards, new FakeTransactionRepository());
        var created = await service.CreateAsync(new CreateCreditCardDto
        {
            Name = "Visa Gold",
            StatementDay = 5,
            PaymentDay = 20,
            CreditLimit = 1000,
            LogoDataUrl = "data:image/png;base64,ABC123",
        });

        var updated = await service.UpdateAsync(created.Id, new UpdateCreditCardDto
        {
            Name = "Visa Gold",
            StatementDay = 5,
            PaymentDay = 20,
            CreditLimit = 1000,
            LogoDataUrl = null,
        });

        Assert.Null(updated.LogoDataUrl);
    }

    [Fact]
    public async Task CreateAsync_guarda_el_nombre_del_banco()
    {
        var service = CreateService(new FakeCreditCardRepository(), new FakeTransactionRepository());

        var created = await service.CreateAsync(new CreateCreditCardDto
        {
            Name = "Visa Gold",
            Bank = "Bancolombia",
            StatementDay = 5,
            PaymentDay = 20,
            CreditLimit = 1000,
        });

        Assert.Equal("Bancolombia", created.Bank);
    }

    [Fact]
    public async Task UpdateAsync_permite_reemplazar_o_quitar_el_banco()
    {
        var creditCards = new FakeCreditCardRepository();
        var service = CreateService(creditCards, new FakeTransactionRepository());
        var created = await service.CreateAsync(new CreateCreditCardDto
        {
            Name = "Visa Gold",
            Bank = "Bancolombia",
            StatementDay = 5,
            PaymentDay = 20,
            CreditLimit = 1000,
        });

        var updated = await service.UpdateAsync(created.Id, new UpdateCreditCardDto
        {
            Name = "Visa Gold",
            Bank = null,
            StatementDay = 5,
            PaymentDay = 20,
            CreditLimit = 1000,
        });

        Assert.Null(updated.Bank);
    }
}
