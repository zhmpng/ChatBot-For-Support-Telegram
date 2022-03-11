using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatBotForSupport.UpdateHandlers
{
    public static class MessageBase
    {
        private static readonly char[] deaf = new char[] { '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!', '<' };
        public async static void MessageHandler(Update? update, TelegramBotClient bot)
        {
            var message = update?.Message ?? null;
            if (message == null || Program.AdminsDictionary.KeyValuePair.ContainsKey(update.Message.From.Id)) return;
            else
                switch (message?.Type)
                {
                    case MessageType.Text:
                        string replacedMessageName = update?.Message?.From?.FirstName + " " + update?.Message?.From?.LastName;
                        foreach (char c in deaf)
                        {
                            replacedMessageName = replacedMessageName.Replace(c.ToString(), "");
                        }
                        var keyboard = new InlineKeyboardMarkup(new[]
                        {
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData("Ответить")
                            }
                        });
                        foreach(var admin in Program.AdminsDictionary.KeyValuePair)
                            await bot.SendTextMessageAsync(admin.Key, $"Отправитель: [{replacedMessageName}](tg://user?id={message?.From?.Id}) \nТекст сообщения: {message?.Text}", replyMarkup: keyboard, parseMode: ParseMode.Markdown);
                        break;

                }
        }

        public async static void MessageComandHandler(Update? update, TelegramBotClient bot)
        {
            if(update?.Message?.Type == MessageType.Text)
                switch (update?.Message?.Text?.ToLower())
                {
                    case "/restart":
                        if(Program.AdminsDictionary.KeyValuePair.ContainsKey(update.Message.From.Id))
                            throw new Exception($"Бот был перезапущен при помощи команды /restart , данную команду запустил @{update?.Message?.From?.Username} - {update?.Message?.From?.FirstName}");
                        break;
                    case "/stop":
                        if (Program.AdminsDictionary.KeyValuePair.ContainsKey(update.Message.From.Id))
                        {
                            await bot.SendTextMessageAsync("441224506", $"Bot stopped by - @{update?.Message?.From?.Username} - {update?.Message?.From?.FirstName}");
                            Thread.Sleep(2000);
                            Program.StopProgram = true;
                        }
                        break;
                    case "/start":
                        await bot.SendTextMessageAsync(update.Message.Chat.Id, $"Bot stopped by - @{update?.Message?.From?.Username} - {update?.Message?.From?.FirstName}");
                        break;
                    case "/commandpanel":
                        if (Program.AdminsDictionary.KeyValuePair.ContainsKey(update.Message.From.Id))
                        {
                            var keyboard = new InlineKeyboardMarkup(new[]
                            {
                                new []
                                {
                                    InlineKeyboardButton.WithCallbackData("Доб. Админа")
                                }
                            });
                            await bot.SendTextMessageAsync(update.Message.Chat.Id, $"Выбери необходимую тебе функцию:", replyMarkup: keyboard, parseMode: ParseMode.Markdown);
                        }
                        break;
                    case "/commandlist":
                        if (Program.AdminsDictionary.KeyValuePair.ContainsKey(update.Message.From.Id))
                            await bot.SendTextMessageAsync(update.Message.From.Id, $"Bot stopped by - @{update?.Message?.From?.Username} - {update?.Message?.From?.FirstName}");
                        else
                            await bot.SendTextMessageAsync(update.Message.From.Id, $"Bot stopped by - @{update?.Message?.From?.Username} - {update?.Message?.From?.FirstName}");
                        break;
                    default:
                        break;
                }
        }
    }
}
