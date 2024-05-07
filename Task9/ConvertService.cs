using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Task9;

public class ConvertService
{
    public ConvertService(string foreignCurrency, DateOnly date)
    {
        ForeignCurrency = foreignCurrency;
        Date = date;
    }
    
    public DateOnly Date { get; }
    public string ForeignCurrency { get; }

    public class PBResponse
    {
        [JsonProperty("exchangeRate")]
        public List<ExchangeRate> ExchangeRates {get;set;}
    }
    public class ExchangeRate
    {
        [JsonProperty("currency")]
        public string Currency {get;set;}
        
        [JsonProperty("saleRateNB")]
        public decimal SaleRate {get;set;}
        
        [JsonProperty("purchaseRateNB")]
        public decimal PurchaseRate {get;set;}
    }
    
    public async Task GetUAHExchangeValue()
    {
        using var client = new HttpClient();
        var kek = await client.GetStringAsync("https://api.privatbank.ua/p24api/exchange_rates?date=01.12.2014");
        
        var stream = await client.GetStreamAsync("https://api.privatbank.ua/p24api/exchange_rates?date=01.12.2014");
        var json =  JsonSerializer.Deserialize<PBResponse>(kek);
        var json2 = JsonConvert.DeserializeObject<PBResponse>(kek);
        Console.WriteLine(json);
    }
}