namespace KatameApi.DTOs.Finance;

public class CreditCardDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StatementDay { get; set; }
    public int PaymentDay { get; set; }
    public decimal CreditLimit { get; set; }
}
