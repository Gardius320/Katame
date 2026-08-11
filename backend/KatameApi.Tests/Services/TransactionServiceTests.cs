using AutoMapper;
using KatameApi.DTOs.Finance;
using KatameApi.Middleware;
using KatameApi.Repositories;
using KatameApi.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace KatameApi.Tests.Services;

public class TransactionServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<FinanceMappingProfile>(), NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    private static TransactionService CreateService(FakeCreditCardRepository? creditCardRepository = null) =>
        new(new FakeTransactionRepository(), creditCardRepository ?? new FakeCreditCardRepository(), CreateMapper());

    private static CreateTransactionDto Sample(
        decimal amount, string type, string category, DateTime date, int? creditCardId = null) => new()
    {
        Amount = amount,
        Type = type,
        Category = category,
        Date = date,
        CreditCardId = creditCardId,
    };

    [Fact]
    public async Task GetPagedAsync_filtra_por_categoria()
    {
        var service = CreateService();
        await service.CreateAsync(Sample(100, "expense", "Comida", new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)));
        await service.CreateAsync(Sample(50, "expense", "Transporte", new DateTime(2026, 8, 2, 0, 0, 0, DateTimeKind.Utc)));

        var result = await service.GetPagedAsync(new TransactionFilter { Category = "Comida" }, 1, 20);

        Assert.Single(result.Items);
        Assert.Equal("Comida", result.Items[0].Category);
    }

    [Fact]
    public async Task GetPagedAsync_pagina_correctamente()
    {
        var service = CreateService();
        for (var i = 1; i <= 5; i++)
        {
            await service.CreateAsync(Sample(10 * i, "expense", "Varios", new DateTime(2026, 8, i, 0, 0, 0, DateTimeKind.Utc)));
        }

        var page1 = await service.GetPagedAsync(new TransactionFilter(), 1, 2);
        var page2 = await service.GetPagedAsync(new TransactionFilter(), 2, 2);

        Assert.Equal(5, page1.TotalCount);
        Assert.Equal(2, page1.Items.Count);
        Assert.Equal(2, page2.Items.Count);
        Assert.NotEqual(page1.Items[0].Id, page2.Items[0].Id);
    }

    [Fact]
    public async Task DeleteAsync_lanza_ApiException_404_si_no_existe()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<ApiException>(() => service.DeleteAsync(999));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task ExportToCsvAsync_genera_encabezado_y_filas()
    {
        var service = CreateService();
        await service.CreateAsync(Sample(100, "income", "Salario", new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)));

        var csv = await service.ExportToCsvAsync(new TransactionFilter());

        Assert.StartsWith("Id,Amount,Type,Category,Date", csv);
        Assert.Contains("Salario", csv);
    }

    [Fact]
    public async Task CreateAsync_lanza_ApiException_404_si_la_tarjeta_no_existe()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            service.CreateAsync(Sample(100, "expense", "Comida", new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc), creditCardId: 999)));

        Assert.Equal(System.Net.HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_vincula_la_tarjeta_cuando_existe_y_GetPagedAsync_filtra_por_ella()
    {
        var creditCardRepository = new FakeCreditCardRepository();
        var card = new KatameApi.Models.CreditCard { Name = "Visa", StatementDay = 5, PaymentDay = 15, CreditLimit = 1000 };
        await creditCardRepository.AddAsync(card);
        var service = CreateService(creditCardRepository);

        var created = await service.CreateAsync(
            Sample(100, "expense", "Comida", new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc), creditCardId: card.Id));
        await service.CreateAsync(Sample(50, "expense", "Transporte", new DateTime(2026, 8, 2, 0, 0, 0, DateTimeKind.Utc)));

        Assert.Equal(card.Id, created.CreditCardId);

        var result = await service.GetPagedAsync(new TransactionFilter { CreditCardId = card.Id }, 1, 20);

        Assert.Single(result.Items);
        Assert.Equal("Comida", result.Items[0].Category);
    }
}
