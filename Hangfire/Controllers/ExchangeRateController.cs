using Hangfire;
using HangfireWithNet9.Services;
using Microsoft.AspNetCore.Mvc;

namespace HangfireWithNet9.Controllers;

[Route("api/exchangerates")]
[ApiController]
public class ExchangeRateController : ControllerBase
{
    private readonly ExchangeRateService _exchangeRateService;

    public ExchangeRateController(ExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }

    // API’den güncel döviz kurlarını döndür
    [HttpGet]
    public async Task<IActionResult> GetExchangeRates()
    {
        var rates = await _exchangeRateService.GetStoredExchangeRates();
        return Ok(rates);
    }

    // API’yi çağırdığında Hangfire işlemi çalışacaktır.
    [HttpPost("start-job")]
    public IActionResult StartExchangeRateJob()
    {
        // Hangfire job ile ExchangeRateService'in metodunu çağırıyoruz
        BackgroundJob.Enqueue<ExchangeRateService>(service => service.FetchAndUpdateRates());

        //json olarak static listeyi verir.
        return Ok(_exchangeRateService.GetStoredExchangeRates());
    }

}