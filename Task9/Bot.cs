using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Task9;

public class Bot
{
    private ITelegramBotClient _botClient;
    private ReceiverOptions _receiverOptions;

    public Bot(string token)
    {
        _botClient = new TelegramBotClient(token);
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message,
                UpdateType.CallbackQuery
            }
        };

    }

    public async Task Start()
    {
        using var cts = new CancellationTokenSource();
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);
        var botInfo = await _botClient.GetMeAsync();
        Console.WriteLine($"{botInfo.FirstName} STARTED!!!!!");
        
        await Task.Delay(-1, cts.Token);
    }

    private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    var message = update.Message;
                    if (message== null) return;
                    var user = message.From;

                    switch (message.Type)
                    {
                        
                    }
                    

                    // Console.WriteLine("MEssage incoming!");
                    // Console.WriteLine(update.Message.Text);
                    // return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    
    private Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };
        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
    
}