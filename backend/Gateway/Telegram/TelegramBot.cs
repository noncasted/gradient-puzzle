using Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace Gateway;

public class TelegramBot : IHostedService, IUpdateHandler
{
    public TelegramBot(TelegramOptions options, ILogger<TelegramBot> logger)
    {
        _logger = logger;
        _bot = new TelegramBotClient(options.Token);
    }

    private const string _gameShortName = "gradient";
    private const string _gameUrl = "https://gradients.ink/";


    private readonly ILogger<TelegramBot> _logger;
    private readonly TelegramBotClient _bot;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var me = await _bot.GetMe(cancellationToken: cancellationToken);

        _logger.LogInformation("Bot started with id {Id}", me.Id);

        _bot.StartReceiving(this, cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
            {
                var message = update.Message!;

                switch (message.Text)
                {
                    case "/help":
                        await _bot.SendMessage(
                            message.Chat.Id,
                            "This bot implements a simple game. Say /game if you want to play.",
                            cancellationToken: cancellationToken);
                        break;
                    case "/start":
                    case "/game":
                        await _bot.SendGame(message.Chat.Id, _gameShortName, cancellationToken: cancellationToken);
                        break;
                }

                break;
            }
            case UpdateType.CallbackQuery:
            {
                var callbackQuery = update.CallbackQuery!;

                if (callbackQuery.GameShortName != _gameShortName)
                {
                    await _bot.AnswerCallbackQuery(
                        callbackQuery.Id,
                        $"Sorry, '{callbackQuery.GameShortName}' is not available.",
                        cancellationToken: cancellationToken);
                }
                else
                {
                    var userId = callbackQuery.From.Id.ToGuid();
                    var url = $"{_gameUrl}?userId={userId}";
                    
                    await _bot.AnswerCallbackQuery(
                        callbackQuery.Id,
                        url: url,
                        cancellationToken: cancellationToken);
                }

                break;
            }
            case UpdateType.InlineQuery:
            {
                var inlineQuery = update.InlineQuery!;

                await _bot.AnswerInlineQuery(inlineQuery.Id,
                    [
                        new InlineQueryResultGame("0", _gameShortName)
                    ],
                    cancellationToken: cancellationToken);
                break;
            }
        }
    }

    public Task HandleErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error in Telegram bot");
        return Task.CompletedTask;
    }
}