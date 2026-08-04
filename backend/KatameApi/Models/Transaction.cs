namespace KatameApi.Models;

public class Transaction : BaseEntity
{
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public static class TransactionType
{
    public const string Income = "income";
    public const string Expense = "expense";

    public static readonly string[] All = { Income, Expense };
}
