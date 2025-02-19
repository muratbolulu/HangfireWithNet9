namespace HangfireWithNet9.Dtos;

public class CurrencyRate
{
    public string CurrencyCode { get; set; } // USD, EUR, GBP vb.
    public decimal Rate { get; set; } // Döviz kuru
}
