using ChatBotForSupport.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        public async static Task MessageHandlerAsync(Update? update, TelegramBotClient bot)
        {
            var message = update?.Message ?? null;
            string replacedMessageName = GetUserName(update);
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Ответить")
                }
            });
            if (message == null || Program.AdminsDictionary.KeyValuePair.ContainsKey(update.Message.From.Id)) return;
            else
                switch (message?.Type)
                {
                    case MessageType.Text:

                        foreach (var admin in Program.AdminsDictionary.KeyValuePair)
                        {
                            Message newMessage = await bot.SendTextMessageAsync(admin.Key, $"Обращение от: [{replacedMessageName}](tg://user?id={message?.From?.Id}) \nТекст обращения: {message?.Text}", replyMarkup: keyboard, parseMode: ParseMode.Markdown);
                            Program.MessageDictionary.AddOrUpdate(newMessage.MessageId, new MessageDictionary() { UserId = message.From.Id, UserMessageId = message.MessageId});
                        }

                        break;
                    case MessageType.Photo:
                        var photoData = await bot.GetFileAsync(message.Photo[message.Photo.Count() - 1].FileId);
                        string filePatch = $"{Directory.GetCurrentDirectory()}\\{photoData.FilePath.Split(@"/").LastOrDefault()}";
                        FileStream photoStream = new FileStream(filePatch, FileMode.OpenOrCreate);
                        await bot.DownloadFileAsync(photoData.FilePath, photoStream);
                        photoStream.Position = 0;
                        foreach (var admin in Program.AdminsDictionary.KeyValuePair)
                        {
                            Message newMessage = await bot.SendPhotoAsync(
                                chatId: admin.Key,
                                photo: photoStream, $"Обращение от: [{replacedMessageName}](tg://user?id={message?.From?.Id}) \nТекст обращения: {message?.Caption}",
                                replyMarkup: keyboard,
                                parseMode: ParseMode.Markdown
                            );
                        Program.MessageDictionary.AddOrUpdate(newMessage.MessageId, new MessageDictionary() { UserId = message.From.Id, UserMessageId = message.MessageId });
                        }
                        photoStream.Close();
                        System.IO.File.Delete(filePatch);
                        break;
                    case MessageType.Document:
                        var docData = await bot.GetFileAsync(message.Document.FileId);
                        string docPatch = $"{Directory.GetCurrentDirectory()}\\{docData.FilePath.Split(@"/").LastOrDefault()}";
                        FileStream docStream = new FileStream(docPatch, FileMode.OpenOrCreate);
                        await bot.DownloadFileAsync(docData.FilePath, docStream);
                        docStream.Position = 0;
                        Telegram.Bot.Types.InputFiles.InputOnlineFile iof = new Telegram.Bot.Types.InputFiles.InputOnlineFile(docStream);
                        iof.FileName = message.Document.FileName;
                        foreach (var admin in Program.AdminsDictionary.KeyValuePair)
                        {
                            Message newMessage = await bot.SendDocumentAsync(
                                chatId: admin.Key,
                                document: iof,
                                caption: $"Файл от: [{replacedMessageName}](tg://user?id={message?.From?.Id}) \nТекст обращения: {message?.Caption}",
                                replyMarkup: keyboard, 
                                parseMode: ParseMode.Markdown
                            );
                            Program.MessageDictionary.AddOrUpdate(newMessage.MessageId, new MessageDictionary() { UserId = message.From.Id, UserMessageId = message.MessageId });
                        }
                        docStream.Close();
                        System.IO.File.Delete(docPatch);
                        break;
                }
        }

        public async static Task MessageComandHandlerAsync(Update? update, TelegramBotClient bot)
        {
            if(update?.Message?.Type == MessageType.Text)
                switch (update?.Message?.Text?.ToLower())
                {
                    case "/start":
                        await bot.SendTextMessageAsync(update.Message.Chat.Id, $"Привет, друг🤗\n"+
                            "Отправляй свой вопрос/задание в этот чат-бот, напиши срок выполнения и свою цену. \nНаш эксперт ответит тебе в ближайшее время в этом чате.");
                        break;
                    case "/help":
                        if (Program.AdminsDictionary.KeyValuePair.ContainsKey(update.Message.From.Id))
                        {
                            var keyboard = new InlineKeyboardMarkup(new[]
                            {
                                new []
                                {
                                    InlineKeyboardButton.WithCallbackData("Add Admin"),
                                    InlineKeyboardButton.WithCallbackData("Remove Admin"),
                                },
                                new []
                                {
                                    InlineKeyboardButton.WithCallbackData("Restart"),
                                    InlineKeyboardButton.WithCallbackData("Stop"),
                                }
                            });
                            await bot.SendTextMessageAsync(update.Message.Chat.Id, $"Выбери необходимую тебе функцию:", replyMarkup: keyboard, parseMode: ParseMode.Markdown);
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(update.Message.From.Id, $"Отправляй свой вопрос/задание в этот чат-бот, напиши срок выполнения и свою цену. \nНаш эксперт ответит тебе в ближайшее время в этом чате.");
                        }
                        break;
                    default:
                        break;
                }
        }

        public async static Task AdminResponseMessageHandlerAsync(Update? update, TelegramBotClient bot)
        {
            string replacedMessageName = GetUserName(update);
            switch (update?.Message?.Type)
            {
                case MessageType.Text:
                    var modeData = Program.AnswerModeDictionary.GetById(update.Message.From.Id);
                    Program.AnswerModeDictionary.Delete(update.Message.From.Id);
                    await bot.DeleteMessageAsync(update.Message.From.Id, modeData.ResponseNotificationId);
                    var requestData = Program.MessageDictionary.GetById(modeData.InlineMessageId);
                    await bot.SendTextMessageAsync(requestData.UserId, $"Ответ эксперта: \n {update.Message.Text}", replyToMessageId: requestData.UserMessageId);
                    break;
            }

        }

        private static string GetUserName(Update? update)
        {
            string replacedMessageName = update?.Message?.From?.FirstName + " " + update?.Message?.From?.LastName;
            foreach (char c in deaf)
            {
                replacedMessageName = replacedMessageName.Replace(c.ToString(), "");
            }
            return replacedMessageName;
        }
    }
}
