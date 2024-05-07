namespace Task9;

class Program
{
    static async Task Main()
    {
        // var bot = new Bot("7109512993:AAGt9rFAfqkVeN9OIdBKRXbmv2xCfWtiSzc");
        // await bot.Start();

        var cs = new ConvertService("USD", new DateOnly(2024, 2, 2));
        await cs.GetUAHExchangeValue();

    }
}