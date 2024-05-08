using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Task9;

public class ConvertService
{
    public ConvertService(DateOnly date)
    {
        Date = date;
    }
    
    public DateOnly Date { get; }
    private List<ExchangeRate>? _exchangeRates;
    
    public async Task<ExchangeRate> GetUahExchangeValue(string foreignCurrency)
    {
        if (_exchangeRates == null)
        {
            await RequestExchangeRates();
        }

        var exchangeRate = _exchangeRates.Find(rate => rate.Currency.Equals(foreignCurrency, StringComparison.CurrentCultureIgnoreCase));
        if (exchangeRate == null)
        {
            throw new ArgumentException("Given currency is not valid");
        }

        return exchangeRate;
    }

    private async Task RequestExchangeRates()
    {
        using var client = new HttpClient();
        var url = $"https://api.privatbank.ua/p24api/exchange_rates?date={Date.ToString("dd.MM.yyyy")}";
        var response = await client.GetStringAsync(url);
        var pbResponse = JsonConvert.DeserializeObject<PBResponse>(response);
        if (pbResponse == null)
        {
            throw new ApplicationException("Error in the API");
        }
        _exchangeRates = pbResponse.ExchangeRates;
    }
}

public class PBResponse
{
    [JsonProperty("exchangeRate")]
    public List<ExchangeRate> ExchangeRates {get;set;}
}

public class ExchangeRate
{
    [JsonProperty("currency")]
    public string Currency {get;set;}
        
    [JsonProperty("saleRate")]
    public decimal SaleRate {get;set;}
        
    [JsonProperty("purchaseRate")]
    public decimal PurchaseRate {get;set;}
}