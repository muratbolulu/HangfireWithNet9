using HangfireWithNet9.Dtos;
using System.Text.Json;
using System.Xml.Linq;

namespace HangfireWithNet9.Services;

public class ExchangeRateService
{
    private readonly HttpClient _httpClient;

    private static List<CurrencyRate> _currentRates = new(); // Static liste

    public ExchangeRateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task FetchAndUpdateRates()
    {
        try
        {
            string tcmbUrl = "https://www.tcmb.gov.tr/kurlar/today.xml";
            var response = await _httpClient.GetStringAsync(tcmbUrl);
            var xmlDoc = XDocument.Parse(response);

            var exchangeRates = xmlDoc.Descendants("Currency")
                .Where(x => x.Attribute("CurrencyCode") != null)
                .Select(x => new CurrencyRate
                {
                    CurrencyCode = x.Attribute("CurrencyCode")?.Value,
                    Rate = decimal.Parse(x.Element("ForexBuying")?.Value ?? "0")
                })
                .ToList();

            if (exchangeRates.Count > 0)
            {
                _currentRates = exchangeRates; // Listeyi güncelle
                Console.WriteLine($"✅ {_currentRates.Count} adet kur bilgisi güncellendi!");
            }
            else
            {
                Console.WriteLine("⚠️ Döviz kuru verisi alınamadı!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata oluştu: {ex.Message}");
        }
    }

    // Kaydedilmiş kurları döndür/kullan
    public Task<List<CurrencyRate>> GetStoredExchangeRates()
    {
        return Task.FromResult(_currentRates);
    }
}