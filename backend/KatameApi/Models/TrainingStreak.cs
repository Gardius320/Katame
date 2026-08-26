namespace KatameApi.Models;

// Una fila por usuario, igual que FinancialProfile -- guarda solo el récord
// histórico. La racha ACTUAL no se guarda aquí: se calcula al vuelo a partir de
// TrainingCompletion + los días de entrenamiento planeados (ver
// TrainingService.CalculateStreakAsync), porque si se guardara como un simple
// contador quedaría desincronizada apenas el usuario cambie su plan semanal.
public class TrainingStreak : IUserOwned
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LongestStreakDays { get; set; }
}
