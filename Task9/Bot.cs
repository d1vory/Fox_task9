using System.Text.RegularExpressions;
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
                    switch (message.Type)
                    {
                        case MessageType.Text:
                            switch (message.Text)
                            {
                                case "/start":
                                    await StartCommand(message);
                                    break;
                                case "/currencies":
                                    await CurrenciesCommand(message);
                                    break;
                                case { } s when s.StartsWith("er", StringComparison.CurrentCultureIgnoreCase):
                                    await GetExchangeRates(message);
                                    break;
                            }
                            break;
                    }

                    return;
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

    private async Task StartCommand(Message message)
    {
        await _botClient.SendTextMessageAsync(
            message.Chat.Id,
            "Hello this bot can help you convert foreign currency to UAH! \n" +
            "To get exchange rates, type <b>'er `dd.MM.YYYY` `currency`'</b> \n" +
            "For example <b>'er 25.04.2024 USD'</b> \n" +
            "You can see list of supported currencies with the command /currencies",
            parseMode:ParseMode.Html
            );
        return;
    }
    
    private async Task CurrenciesCommand(Message message)
    {
        await _botClient.SendTextMessageAsync(
            message.Chat.Id,
            "Supported currenices are:\n" +
            "USD\tдолар США\nEUR\tєвро\nCHF\tшвейцарський франк\nGBP\tбританський фунт\nPLZ\tпольський злотий\nSEK\tшведська крона\nXAU\tзолото\nCAD\tканадський долар"
        );
        return;
    }

    private async Task GetExchangeRates(Message message)
    {
        try
        {
            var cs = new ConvertService(message.Text);
            var exchangeRate = await cs.GetUahExchangeValue();
            await _botClient.SendTextMessageAsync(
                message.Chat.Id,
                $"Exchange rates for <b>{exchangeRate.Currency}</b> on {cs.Date} are:\n" +
                $"Purchase: <b>{decimal.Round(exchangeRate.PurchaseRate, 2)}</b>\n" +
                $"Sale: <b>{decimal.Round(exchangeRate.SaleRate, 2)}</b>",
                parseMode:ParseMode.Html
            );
        }
        catch (ArgumentException ex)
        {
            await _botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Input is not recognized, please use format <b>'er `dd.MM.YYYY` `currency`'</b>\n" +
                "For example <b>'er 25.04.2024 USD'</b> \n",
                parseMode: ParseMode.Html
            );
        }
        catch (ApplicationException ex)
        {
            await _botClient.SendTextMessageAsync(
                message.Chat.Id,
                ex.Message
            );
        }
    }
    
}