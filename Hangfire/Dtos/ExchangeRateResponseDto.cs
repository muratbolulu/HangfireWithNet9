namespace HangfireWithNet9.Dtos
{
    public class ExchangeRateResponseDto
    {
        public Dictionary<string, decimal> Rates { get; set; } = new();
    }
}
