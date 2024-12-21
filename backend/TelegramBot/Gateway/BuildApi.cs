using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace Gateway;

public class BuildApi : IHostedService, IUpdateHandler
{
    public BuildApi(SecretsOptions options)
    {
        _bot = new TelegramBotClient(options.Token);
    }

    private const string _gameShortName = "gradient";

    private readonly TelegramBotClient _bot;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var me = await _bot.GetMe(cancellationToken: cancellationToken);
        Console.WriteLine(me.Id);

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
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message!;
            if (message.Text == "/help")
            {
                await _bot.SendMessage(
                    message.Chat.Id,
                    "This bot implements a simple game. Say /game if you want to play."
                );
            }
            else if (message.Text == "/start" || message.Text == "/game")
            {
                await _bot.SendGame(message.Chat.Id, _gameShortName, cancellationToken: cancellationToken);
            }
        }

        if (update.Type == UpdateType.CallbackQuery)
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
                var gameUrl = $"https://gradient-puzzle.noncasted.dev/";
                await _bot.AnswerCallbackQuery(callbackQuery.Id, url: gameUrl, cancellationToken: cancellationToken);
            }
        }

        if (update.Type == UpdateType.InlineQuery)
        {
            var inlineQuery = update.InlineQuery!;
            await _bot.AnswerInlineQuery(inlineQuery.Id, [
                new InlineQueryResultGame("0", _gameShortName)
            ], cancellationToken: cancellationToken);
        }
    }

    public Task HandleErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }
}