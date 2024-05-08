using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Telegram.Bot.Types.Enums;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Task9;

public class ConvertService
{
        
    public DateOnly Date { get; private set; }
    public string Currency { get; private set; }
    private List<ExchangeRate>? _exchangeRates;
    
    public ConvertService(string input)
    {
        var pattern = @"\s*[erER]{2}\s*(\d{1,2}[\.\/]\d{1,2}[\.\/]\d{4})\s*(\w{3})";
        var match = Regex.Match(input, pattern);
        if (!match.Success)
        {
            throw new ArgumentException("Input is not valid, use 'er `dd.MM.YYYY` `currency`' format");
        }

        var isDateValid = DateOnly.TryParse(match.Groups[1].ToString(), out var parsedDate);
        if (!isDateValid)
        {
            throw new ApplicationException("Given date is not valid");
        }

        Date = parsedDate;
        Currency = match.Groups[2].ToString();

        string[] allowedCurrencies = ["USD", "EUR", "CHF", "GBP", "PLZ", "SEK", "XAU", "CAD"];
        if (!allowedCurrencies.Any(c => c.Equals(Currency, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new ApplicationException("Given currency is not valid");
        }
    }

    public async Task<ExchangeRate> GetUahExchangeValue()
    {
        if (_exchangeRates == null)
        {
            await RequestExchangeRates();
        }

        var exchangeRate = _exchangeRates.Find(rate => rate.Currency.Equals(Currency, StringComparison.CurrentCultureIgnoreCase));
        if (exchangeRate == null)
        {
            throw new ApplicationException("Exchange rate for given currency and date is not found!");
        }

        return exchangeRate;
    }

    private async Task RequestExchangeRates()
    {
        var client = Program.Client;
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
    public decimal? SaleRatePB {get;set;}
        
    [JsonProperty("purchaseRate")]
    public decimal? PurchaseRatePB {get;set;}
    
    [JsonProperty("saleRateNB")]
    public decimal SaleRateNB {get;set;}
        
    [JsonProperty("purchaseRateNB")]
    public decimal PurchaseRateNB {get;set;}

    public decimal SaleRate => SaleRatePB ?? SaleRateNB;
    public decimal PurchaseRate => PurchaseRatePB ?? PurchaseRateNB;
}