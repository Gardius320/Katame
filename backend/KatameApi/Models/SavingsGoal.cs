namespace KatameApi.Models;

public class SavingsGoal : IUserOwned
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? DueDate { get; set; }
    // Cuánto planea aportar el usuario a esta meta cada mes (opcional). Se usa junto
    // con FinancialProfile.MonthlyIncome para mostrar qué porcentaje de su ingreso
    // representa. No tiene nada que ver con CurrentAmount/TargetAmount (el total
    // acumulado); es solo el plan mensual.
    public decimal? MonthlyContributionTarget { get; set; }

    // Racha de meses seguidos aportando a esta meta (independiente para cada
    // meta, igual que MonthlyContributionTarget). Se recalcula en cada aporte
    // comparando el mes de hoy contra LastContributionMonth -- ver
    // SavingsGoalService.ContributeAsync.
    public int CurrentStreakMonths { get; set; }
    public int LongestStreakMonths { get; set; }
    public DateTime? LastContributionMonth { get; set; }
}
