using ChatBotForSupport.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatBotForSupport.UpdateHandlers
{
    public static class CallbackBase
    {
        public async static Task CallbackHandlerAsync(Update update, TelegramBotClient bot)
        {
            var callbackQuery = update.CallbackQuery;
            switch (callbackQuery.Data)
            {
                case "Ответить":
                    var keyboard = new InlineKeyboardMarkup(new[]
                         {
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData("Отменить ответ.")
                            }
                    });
                    var responseNotification = await bot.SendTextMessageAsync(update.CallbackQuery.From.Id, $"Введи сообщение для отправки пользователю🔽", replyMarkup: keyboard, parseMode: ParseMode.Markdown);
                    Program.AnswerModeDictionary.AddOrUpdate(update.CallbackQuery.From.Id, new AnswerModeDictionary() { InlineMessageId = callbackQuery.Message.MessageId, ResponseNotificationId = responseNotification.MessageId });
                    break;
                case "Add Admin":
                    await bot.SendTextMessageAsync(update.CallbackQuery.From.Id, $"Comming soon... 🤗");
                    await bot.DeleteMessageAsync(update.CallbackQuery.From.Id, callbackQuery.Message.MessageId);
                    break;
                case "Отменить ответ":
                    var modeData = Program.AnswerModeDictionary.GetById(update.CallbackQuery.From.Id);
                    Program.AnswerModeDictionary.Delete(update.CallbackQuery.From.Id);
                    await bot.DeleteMessageAsync(update.CallbackQuery.From.Id, modeData.ResponseNotificationId);
                    break;
                case "Restart":
                    await bot.SendTextMessageAsync(update.CallbackQuery.From.Id, $"Бот был перезапущен при помощи команды /restart , данную команду запустил @{update?.CallbackQuery?.From?.Username} - {update?.CallbackQuery?.From?.FirstName}");
                    throw new Exception($"Бот был перезапущен при помощи команды /restart , данную команду запустил @{update?.CallbackQuery?.From?.Username} - {update?.CallbackQuery?.From?.FirstName}");
                case "Stop":
                    foreach (var admin in Program.AdminsDictionary.KeyValuePair)
                        await bot.SendTextMessageAsync(admin.Key, $"Bot stopped by - @{update?.CallbackQuery?.From?.Username} - {update?.CallbackQuery?.From?.FirstName}");
                    Process.GetCurrentProcess().Kill();
                    break;
                case "Remove Admin":
                    await bot.SendTextMessageAsync(update.CallbackQuery.From.Id, $"Comming soon... 🤗");
                    await bot.DeleteMessageAsync(update.CallbackQuery.From.Id, callbackQuery.Message.MessageId);
                    break;
            }
        }
    }
}
