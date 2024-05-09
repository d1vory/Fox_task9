using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Telegram.Bot.Types.Enums;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Task9;

public class ConvertService
{
        
    public DateOnly Date { get; private set; }
    public string Currency { get; private set; }
    public static readonly string[] AllowedCurrencies = [
        "USD", "EUR", "CHF", "GBP", "PLZ", "SEK", "XAU", "CAD"
    ];
    
    private List<ExchangeRate>? _exchangeRates;
    private readonly HttpClient _сlient = new HttpClient();

    public ConvertService(string input)
    {
        if (!CheckInput(input, out var stringDate, out var parsedCurrency))
        {
            throw new ArgumentException("Input is not valid, use 'er `dd.MM.YYYY` `currency`' format");
        }
        
        Date = CheckDate(stringDate);
        Currency = CheckCurrency(parsedCurrency);
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

    public static bool CheckInput(string input, out string date, out string currency)
    {
        var pattern = @"\s*[erER]{2}\s*(\d{1,2}[\.\/]\d{1,2}[\.\/]\d{4})\s*(\w{3})";
        var match = Regex.Match(input, pattern);
        if (match.Success && match.Groups.Count == 3)
        {
            date = match.Groups[1].ToString();
            currency = match.Groups[2].ToString();
            return true;
        }

        date = "";
        currency = "";
        return false;
    }

    public static DateOnly CheckDate(string inputDate)
    {
        var isDateValid = DateOnly.TryParse(inputDate, out var parsedDate);
        if (!isDateValid)
        {
            throw new ApplicationException("Given date is not valid");
        }

        return parsedDate;
    }

    public static string CheckCurrency(string inputCurrency)
    {
        if (!AllowedCurrencies.Any(c => c.Equals(inputCurrency, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new ApplicationException("Given currency is not valid");
        }

        return inputCurrency;
    }

    private async Task RequestExchangeRates()
    {
        var url = $"https://api.privatbank.ua/p24api/exchange_rates?date={Date.ToString("dd.MM.yyyy")}";
        var response = await _сlient.GetStringAsync(url);
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