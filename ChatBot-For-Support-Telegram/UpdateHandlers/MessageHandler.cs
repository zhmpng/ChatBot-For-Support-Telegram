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
        public async static void MessageHandler(Update update, TelegramBotClient bot)
        {
            var message = update.Message ?? null;
            if (message == null) return;
            else
                switch (message?.Type)
                {
                    case MessageType.Text:
                        //await bot.SendTextMessageAsync("441224506", message?.Text);
                        string replacedMessageName = update.Message.From.FirstName + " " + update.Message.From.LastName;
                        foreach (char c in deaf)
                        {
                            replacedMessageName = replacedMessageName.Replace(c.ToString(), "");
                        }
                        var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData("Ответить")
                            }
                        });
                        await bot.SendTextMessageAsync(message.Chat.Id, $"Отправитель: [{replacedMessageName}](tg://user?id={message?.From?.Id}) \nТекст сообщения: {message?.Text}", replyMarkup: keyboard, parseMode: ParseMode.Markdown);
                        break;

                }
                //if (message?.Type == MessageType.Text && message?.Text?.Replace("@BeOpenChatBot", "").ToLower() == ("/restart"))
                //{
                //    repeat = false;
                //    _offset = 0;
                //    throw new Exception($"Бот был перезапущен при помощи команды /restart , данную команду запустил @{message?.From?.Username} - {message?.From?.FirstName}");
                //}
        }
    }
}
