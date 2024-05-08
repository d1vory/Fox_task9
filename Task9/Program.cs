namespace Task9;



class Program
{
    public static readonly HttpClient Client = new HttpClient();
    
    static async Task Main()
    {
        var bot = new Bot("7109512993:AAGt9rFAfqkVeN9OIdBKRXbmv2xCfWtiSzc");
        await bot.Start();
    }
}