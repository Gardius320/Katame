using AutoMapper;
using KatameApi.Models;

namespace KatameApi.DTOs.Finance;

public class FinanceMappingProfile : Profile
{
    public FinanceMappingProfile()
    {
        CreateMap<Transaction, TransactionDto>();
        CreateMap<SavingsGoal, SavingsGoalDto>();
        CreateMap<Obligation, ObligationDto>();
        CreateMap<CreditCard, CreditCardDto>();
        CreateMap<Budget, BudgetDto>();
    }
}
