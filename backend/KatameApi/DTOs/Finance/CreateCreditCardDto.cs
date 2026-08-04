namespace KatameApi.DTOs.Finance;

public class CreateCreditCardDto
{
    public string Name { get; set; } = string.Empty;
    public int StatementDay { get; set; }
    public int PaymentDay { get; set; }
    public decimal CreditLimit { get; set; }
}
