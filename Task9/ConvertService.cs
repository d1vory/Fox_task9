namespace Task9;

public class ConvertService
{
    public ConvertService(string foreignCurrency, decimal inputValue, DateOnly date)
    {
        ForeignCurrency = foreignCurrency;
        InputValue = inputValue;
        Date = date;
    }
    
    public decimal InputValue { get; }
    public DateOnly Date { get; }
    public string ForeignCurrency { get; }

    public decimal GetUAHExchangeValue()
    {
        
    }
    
}