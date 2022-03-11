using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ChatBotForSupport.UpdateHandlers
{
    public sealed class CallbackBase
    {
        public async static void CallbackHandler(Update update, TelegramBotClient bot)
        {
            var callbackQuery = update.CallbackQuery;
            switch (callbackQuery.Data)
            {
                case "Ответить":
                    await bot.SendTextMessageAsync(update.Message.From.Id, $"Введи сообщения которое будет отправлено как ответ🔽");
                    break;
                case "Доб. Админа":
                    break;
            }
        }
    }
}
